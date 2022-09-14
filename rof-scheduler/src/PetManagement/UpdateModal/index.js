import { Modal, Row, Form, Col, Button, Spinner, Alert } from "react-bootstrap";
import "./updatePet.css";
import { useState, useEffect } from "react";
import Select from "react-select";

import { ensurePetUpdateInformationProvided } from "../../SharedServices/inputValidationService";
import {
  updatePetInformation,
  getPetById,
} from "../../SharedServices/petManagementService";

function UpdatePetModal({
  pet,
  vaccines,
  show,
  handleHide,
  postUpdateAction,
  setLoginState,
}) {
  const [validationMap, setValidationMap] = useState(new Map());
  const [updating, setUpdating] = useState(false);
  const [errMsg, setErrMsg] = useState(undefined);
  const [successMsg, setSuccessMsg] = useState(false);
  const [vaccine, setVaccine] = useState([]);

  const resetStates = function () {
    setValidationMap(new Map());
    setErrMsg(undefined);
    setSuccessMsg(undefined);
  };

  const closeModal = function () {
    resetStates();
    handleHide();
  };

  const setVaccineValue = (colIndex, vaccineIndex) => {
    //value equals opposite of what it currently is
    vaccines[colIndex][vaccineIndex].checked =
      !vaccines[colIndex][vaccineIndex].checked;
    setVaccine(vaccines);
  };

  const handleUpdate = (e) => {
    e.preventDefault();

    setErrMsg(undefined);
    setSuccessMsg(false);

    var petName = e.target.petName.value;
    var weight = parseFloat(e.target.weight.value);
    var dob = e.target.dob.value;
    var breedId = pet.breedId;
    var ownerId = pet.ownerId;
    var otherInfo = e.target.otherInfo.value;

    var vaccineStatus = undefined;

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
    } else {
      setValidationMap(new Map());
      setUpdating(true);

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
    }
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

              <Form.Group as={Col} lg={3}>
                <Form.Label>Breed</Form.Label>
                <Form.Control
                  placeholder="Breed"
                  name="breed"
                  defaultValue={pet === undefined ? "" : pet.breedName}
                  isInvalid={validationMap.has("breed")}
                  disabled
                />
                <Form.Control.Feedback type="invalid">
                  {validationMap.get("breed")}
                </Form.Control.Feedback>
              </Form.Group>

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
                    <Form.Control
                      placeholder="Owner"
                      name="client"
                      defaultValue={
                        pet === undefined
                          ? ""
                          : pet.ownerFirstName + " " + pet.ownerLastName
                      }
                      isInvalid={validationMap.has("")}
                      disabled
                    />
                    <Form.Control.Feedback type="invalid">
                      {validationMap.get("owner")}
                    </Form.Control.Feedback>
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
                {
                  //first column
                  vaccines[0].map((vaccine, index) => {
                    return (
                      <Form.Check
                        key={vaccine.id}
                        type="checkbox"
                        label={vaccine.vaxName}
                        value={vaccine.checked}
                        onChange={() => setVaccineValue(0, index)} //first param tells us which column, second param tells us which index value to update
                      />
                    );
                  })
                }
              </Form.Group>
              <Form.Group as={Col} lg={3}>
                {
                  //second column
                  vaccines[1].map((vaccine, index) => {
                    return (
                      <Form.Check
                        key={vaccine.id}
                        type="checkbox"
                        label={vaccine.vaxName}
                        value={vaccine.checked}
                        onChange={() => setVaccineValue(1, index)} //first param tells us which column, second param tells us which index value to update
                      />
                    );
                  })
                }
              </Form.Group>
              <Form.Group as={Col} lg={3}>
                {
                  //third column
                  vaccines[2].map((vaccine, index) => {
                    return (
                      <Form.Check
                        key={vaccine.id}
                        type="checkbox"
                        label={vaccine.vaxName}
                        value={vaccine.checked}
                        onChange={() => setVaccineValue(2, index)} //first param tells us which column, second param tells us which index value to update
                      />
                    );
                  })
                }
              </Form.Group>
              <Form.Group as={Col} lg={3}>
                {
                  //fourth column
                  vaccines[3].map((vaccine, index) => {
                    return (
                      <Form.Check
                        key={vaccine.id}
                        type="checkbox"
                        label={vaccine.vaxName}
                        value={vaccine.checked}
                        onChange={() => setVaccineValue(3, index)} //first param tells us which column, second param tells us which index value to update
                      />
                    );
                  })
                }
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
