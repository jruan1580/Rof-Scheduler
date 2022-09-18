import { Modal, Row, Form, Col, Button, Spinner, Alert } from "react-bootstrap";
import "./updatePet.css";
import { useState, useEffect } from "react";
import Select from "react-select";

import { ensurePetUpdateInformationProvided } from "../../SharedServices/inputValidationService";
import { updatePetInformation, getVaccinesByPetId } from "../../SharedServices/petManagementService";
import { getBreedByPetType, getClients } from "../../SharedServices/dropdownService";

function UpdatePetModal({
  pet,
  show,
  handleHide,
  postUpdateAction,
  setLoginState,
}) {
  const [validationMap, setValidationMap] = useState(new Map());
  const [updating, setUpdating] = useState(false);
  const [errMsg, setErrMsg] = useState(undefined);
  const [successMsg, setSuccessMsg] = useState(false);
  const [breedByPetType, setBreedByPetType] = useState([]);
  const [owners, setOwners] = useState([]);
  const [petVaxes, setPetVaxes] = useState([[], [], [], []]);

  useEffect(() => {
    (async function () {
      try {
        var petTypeId = parseInt(pet.petTypeId);
        var petId = parseFloat(pet.id);

        var resp = await getBreedByPetType(petTypeId);
        if (resp.status === 401) {
          setLoginState(false);
          return;
        }

        const breeds = await resp.json();
        constructBreedOptions(breeds);

        resp = await getClients();
        if (resp.status === 401) {
          setLoginState(false);
          return;
        }

        const clients = await resp.json();
        constructOwnersOption(clients);

        resp = await getVaccinesByPetId(petId);
        if (resp.status === 401) {
          setLoginState(false);
          return;
        }

        const petVaccines = await resp.json();
        constructPetVaccines(petVaccines);

        setErrMsg(undefined);
      } catch (e) {
        setErrMsg(e.message);
      }
    })();
  }, [pet]);

  const resetStates = function () {
    setValidationMap(new Map());
    setErrMsg(undefined);
    setSuccessMsg(undefined);
  };

  const closeModal = function () {
    resetStates();
    handleHide();
  };

  const constructBreedOptions = (breeds) => {
    const breedOptions = [];
    for (var i = 0; i < breeds.length; i++) {
      breedOptions.push({ value: breeds[i].id, label: breeds[i].breedName });
    }

    setBreedByPetType(breedOptions);
  };

  const constructOwnersOption = (owners) => {
    const ownerOptions = [];
    for (var i = 0; i < owners.length; i++) {
      ownerOptions.push({ label: owners[i].fullName, value: owners[i].id });
    }

    setOwners(ownerOptions);
  };

  const constructPetVaccines = (petVaccines) => {
    const vaccinesByCol = [];
    //push 4 empty lists first represent 4 columns of list of vaccines
    vaccinesByCol.push([]);
    vaccinesByCol.push([]);
    vaccinesByCol.push([]);
    vaccinesByCol.push([]);

    //loop through vaccine and push each vaccine into each column.
    //when we hit last column, then reset back to first column
    var col = 0; //start at first column
    for (var i = 0; i < petVaccines.length; i++) {
      const vax = {
        id: petVaccines[i].id,
        vaxName: petVaccines[i].vaccineName,
        checked: petVaccines[i].inoculated,
        petVaccineId: petVaccines[i].petsVaccineId
      }; //not new anymore, so checked value is what's in DB

      vaccinesByCol[col].push(vax);

      col++; //go to next column
      if (col == 4) {
        //when col == 3, that means we were at 4th column already (zero indexed).
        //col ++ will increment to 4 meaning we need to reset back to fist column
        col = 0;
      }
    }

    setPetVaxes(vaccinesByCol);
  };

  const setVaccineValue = (colIndex, vaccineIndex) => {
    //value equals opposite of what it currently is
    petVaxes[colIndex][vaccineIndex].checked =
      !petVaxes[colIndex][vaccineIndex].checked;

    setPetVaxes(petVaxes);
  };

  const handleUpdate = (e) => {
    e.preventDefault();

    setErrMsg(undefined);
    setSuccessMsg(false);

    var petName = e.target.petName.value;
    var weight = parseFloat(e.target.weight.value);
    var dob = e.target.dob.value;
    var breedId = parseFloat(e.target.breed.value);
    var ownerId = parseFloat(e.target.client.value);
    var otherInfo = e.target.otherInfo.value;

    var inputValidations = new Map();

    inputValidations = ensurePetUpdateInformationProvided(
      petName,
      weight,
      dob,
      breedId,
      ownerId
    );

    if (inputValidations.size > 0) {
      setValidationMap(inputValidations);
      return;
    }
    setValidationMap(new Map());
    setUpdating(true);

    const vaccineStatus = [];
    for (var row = 0; row < petVaxes.length; row++) {
      const vaxRow = petVaxes[row];
      for (var col = 0; col < vaxRow.length; col++) {
        vaccineStatus.push({
          id: vaxRow[col].id,
          vaccineName: vaxRow[col].vaxName,
          inoculated: vaxRow[col].checked,
          petsVaccineId: vaxRow[col].petVaccineId
        });
      }
    }

    (async function () {
      try {
        var updatedFields = new Map();
        updatedFields.set("id", pet.id);
        var resp = undefined;

        resp = await updatePetInformation(
          pet.id,
          petName,
          weight,
          dob,
          ownerId,
          breedId,
          otherInfo,
          vaccineStatus
        );

        updatedFields.set("petName", petName);
        updatedFields.set("weight", weight);
        updatedFields.set("dob", dob);
        updatedFields.set("breed", breedId);
        updatedFields.set("client", ownerId);
        updatedFields.set("otherInfo", otherInfo);

        if (resp !== undefined && resp.status === 401) {
          setLoginState(false);
          return;
        }

        postUpdateAction(updatedFields);
        setSuccessMsg(true);
      } catch (e) {
        setErrMsg(e.message);
      } finally {
        setUpdating(false);
      }
    })();
  };

  return (
    <>
      <Modal show={show} onHide={closeModal} dialogClassName="update-modal70">
        <Modal.Header className="update-modal-header-color">
          <Modal.Title id="contained-modal-title-vcenter">
            Update Pet
          </Modal.Title>
        </Modal.Header>

        <Modal.Body>
          <Form onSubmit={handleUpdate}>
            {errMsg !== undefined && <Alert variant="danger">{errMsg}</Alert>}
            {successMsg && (
              <Alert variant="success">Pet successfully updated.</Alert>
            )}
            <h4>Pet Information</h4>
            <br />
            <Row>
              <Form.Group as={Col} lg={3}>
                <Form.Label>Name</Form.Label>
                <Form.Control
                  placeholder="Pet Name"
                  name="petName"
                  defaultValue={pet === undefined ? "" : pet.name}
                  isInvalid={validationMap.has("petName")}
                />
                <Form.Control.Feedback type="invalid">
                  {validationMap.get("petName")}
                </Form.Control.Feedback>
              </Form.Group>

              {localStorage.getItem("role").toLowerCase() !== "client" && (
                <Form.Group as={Col} lg={3}>
                  <Form.Label>Breed</Form.Label>
                  {pet !== undefined && (
                    <Select
                      name="breed"
                      options={breedByPetType}
                      defaultValue={{
                        label: pet.breedName,
                        value: pet.breedId,
                      }}
                      isInvalid={validationMap.has("breed")}
                      style={{ borderColor: "red" }}
                    />
                  )}
                  <div className="dropdown-invalid">
                    {" "}
                    {validationMap.get("breed")}
                  </div>
                </Form.Group>
              )}

              <Form.Group as={Col} lg={3}>
                <Form.Label>DOB</Form.Label>
                <Form.Control
                  type="date"
                  defaultValue={pet === undefined ? "DOB" : pet.dob}
                  name="dob"
                  isInvalid={validationMap.has("dob")}
                />
                <Form.Control.Feedback type="invalid">
                  {validationMap.get("dob")}
                </Form.Control.Feedback>
              </Form.Group>

              <Form.Group as={Col} lg={3}>
                <Form.Label>Weight</Form.Label>
                <Form.Control
                  defaultValue={pet === undefined ? "weight" : pet.weight}
                  name="weight"
                  isInvalid={validationMap.has("weight")}
                />
                <Form.Control.Feedback type="invalid">
                  {validationMap.get("weight")}
                </Form.Control.Feedback>
              </Form.Group>
            </Row>
            <br />

            {localStorage.getItem("role").toLowerCase() !== "client" && (
              <>
                <Row>
                  <Form.Group as={Col} lg={4}>
                    <Form.Label>Owner</Form.Label>
                    {pet !== undefined && (
                      <Select
                        name="client"
                        defaultValue={{
                          label: pet.ownerFirstName + " " + pet.ownerLastName,
                          value: pet.ownerId,
                        }}
                        options={owners}
                      />
                    )}
                    <div className="dropdown-invalid">
                      {" "}
                      {validationMap.get("client")}
                    </div>
                  </Form.Group>
                </Row>
                <br />
              </>
            )}

            <Row>
              <Form.Group as={Col} lg={12}>
                <Form.Label>Additional Information (Optional)</Form.Label>
                <Form.Control
                  placeholder="Additional Info"
                  defaultValue={pet === undefined ? "" : pet.otherInfo}
                  name="otherInfo"
                  as="textarea"
                  rows={5}
                />
              </Form.Group>
            </Row>
            <br />

            <h4>Vaccines</h4>
            <br />

            <Row>
              <Form.Group as={Col} lg={3}>
                {pet !== undefined && petVaxes !== undefined && petVaxes[0].map((vaccine, index) => {
                    return (
                      <Form.Check
                        key={vaccine.id}
                        type="checkbox"
                        label={vaccine.vaxName}
                        defaultChecked={vaccine.checked}
                        onChange={() => setVaccineValue(0, index)} //first param tells us which column, second param tells us which index value to update
                      />
                    );
                  })}
                </Form.Group>

              <Form.Group as={Col} lg={3}>
                {pet !== undefined && petVaxes !== undefined && petVaxes[1].map((vaccine, index) => {
                    return (
                      <Form.Check
                        key={vaccine.id}
                        type="checkbox"
                        label={vaccine.vaxName}
                        defaultChecked={vaccine.checked}
                        onChange={() => setVaccineValue(1, index)} //first param tells us which column, second param tells us which index value to update
                      />
                    );
                  })}
                </Form.Group>

                <Form.Group as={Col} lg={3}>
                {pet !== undefined && petVaxes !== undefined && petVaxes[2].map((vaccine, index) => {
                    return (
                      <Form.Check
                        key={vaccine.id}
                        type="checkbox"
                        label={vaccine.vaxName}
                        defaultChecked={vaccine.checked}
                        onChange={() => setVaccineValue(2, index)} //first param tells us which column, second param tells us which index value to update
                      />
                    );
                  })}
                </Form.Group>

                <Form.Group as={Col} lg={3}>
                {pet !== undefined && petVaxes !== undefined && petVaxes[3].map((vaccine, index) => {
                    return (
                      <Form.Check
                        key={vaccine.id}
                        type="checkbox"
                        label={vaccine.vaxName}
                        defaultChecked={vaccine.checked}
                        onChange={() => setVaccineValue(3, index)} //first param tells us which column, second param tells us which index value to update
                      />
                    );
                  })}
                </Form.Group>
            </Row>
            <hr></hr>
            {updating && (
              <Button
                type="button"
                variant="danger"
                className="float-end ms-2"
                disabled
              >
                Cancel
              </Button>
            )}
            {!updating && (
              <Button
                type="button"
                variant="danger"
                onClick={() => closeModal()}
                className="float-end ms-2"
              >
                Cancel
              </Button>
            )}
            {updating && (
              <Button variant="primary" className="float-end" disabled>
                <Spinner
                  as="span"
                  animation="grow"
                  size="sm"
                  role="status"
                  aria-hidden="true"
                />
                Updating...
              </Button>
            )}
            {!updating && (
              <Button type="submit" className="float-end">
                Update
              </Button>
            )}
          </Form>
        </Modal.Body>
      </Modal>
    </>
  );
}

export default UpdatePetModal;
