import { useState, useEffect } from "react";
import {
  Form,
  Button,
  Row,
  Col,
  Pagination,
  OverlayTrigger,
  Table,
  Tooltip,
  Alert,
} from "react-bootstrap";
import AddPetModal from "./AddModal";
import UpdatePetModal from "./UpdateModal";
import "./index.css";
import {
  getAllPets,
  getPetsByClientId,
} from "../SharedServices/petManagementService";
import { getVaccinesByPetType } from "../SharedServices/dropdownService";

function PetManagement({ setLoginState }) {
  const [userType, setUserType] = useState("");
  const [pets, setPets] = useState([]);
  const [errMsg, setErrMsg] = useState(undefined);
  const [currPage, setCurrPage] = useState(1);
  const [keyword, setKeyword] = useState("");
  const [totalPages, setTotalPages] = useState(0);
  const [showAddModal, setShowAddModal] = useState(false);
  const [showUpdateModal, setShowUpdateModal] = useState(false);
  const [currPetToUpdate, setCurrPetToUpdate] = useState(undefined);
  const [vaccinesByPetType, setVaccinesByPetType] = useState([]);

  useEffect(() => {
    (async function () {
      try {
        var resp = undefined;

        if (localStorage.getItem("role") === "Client") {
          setUserType("Client");
          resp = await getPetsByClientId(currPage, 10, keyword);
        } else {
          setUserType("Employee");
          resp = await getAllPets(currPage, 10, keyword);
        }

        const petByPage = await resp.json();

        setPets(petByPage.pets);
        setTotalPages(petByPage.totalPages);
      } catch (e) {
        setErrMsg(e.message);
      }
    })();
  }, [currPage, keyword]);

  const search = (searchEvent) => {
    searchEvent.preventDefault();
    const searchTerm =
      searchEvent.target.searchPet.value === undefined
        ? ""
        : searchEvent.target.searchPet.value;
    setKeyword(searchTerm);
    setCurrPage(1);
  };

  const loadUpdateModal = (pet) => {
    setCurrPetToUpdate(pet);
    (async function () {
      try {
        //grab vaccines by pet type selected
        var resp = await getVaccinesByPetType(pet.petTypeId);
        if (resp.status === 401) {
          setLoginState(false);
          return;
        }

        const vaccines = await resp.json();
        constructVaccinesByPetType(vaccines);

        setErrMsg(undefined);
      } catch (e) {
        setErrMsg(e.message);
        return;
      }
    })();
    setShowUpdateModal(true);
  };

  const constructVaccinesByPetType = (vaccines) => {
    //on the ui, we are going to break vaccines up into 4 columns
    //so we will evenly split vaccines up into group of 4
    const vaccinesByCol = [];
    //push 4 empty lists first represent 4 columns of list of vaccines
    vaccinesByCol.push([]);
    vaccinesByCol.push([]);
    vaccinesByCol.push([]);
    vaccinesByCol.push([]);

    //loop through vaccine and push each vaccine into each column.
    //when we hit last column, then reset back to first column
    var col = 0; //start at first column
    for (var i = 0; i < vaccines.length; i++) {
      const vax = {
        id: vaccines[i].id,
        vaxName: vaccines[i].vaccineName,
        checked: false,
      }; //since its new, checked is false
      vaccinesByCol[col].push(vax);

      col++; //go to next column
      if (col == 4) {
        //when col == 3, that means we were at 4th column already (zero indexed).
        //col ++ will increment to 4 meaning we need to reset back to fist column
        col = 0;
      }
    }

    setVaccinesByPetType(vaccinesByCol);
  };

  const setVaccineValue = (colIndex, vaccineIndex) => {
    //value equals opposite of what it currently is
    vaccinesByPetType[colIndex][vaccineIndex].checked =
      !vaccinesByPetType[colIndex][vaccineIndex].checked;
    setVaccinesByPetType(vaccinesByPetType);
  };

  const postUpdatePetAction = (updatedFieldsMap) => {
    for (var i = 0; i < pets.length; i++) {
      if (pets[i].id === updatedFieldsMap.get("id")) {
        pets[i].name = updatedFieldsMap.get("petName");
        pets[i].ownerId = updatedFieldsMap.get("client");
        pets[i].breedId = updatedFieldsMap.get("breed");
        break;
      }
    }
    setPets(pets);
  };

  return (
    <>
      <h1>Pets Management</h1>
      <br />
      <AddPetModal
        show={showAddModal}
        handleHide={() => setShowAddModal(false)}
        setLoginState={setLoginState}
      />
      <UpdatePetModal
        pet={currPetToUpdate}
        vaccines={vaccinesByPetType}
        show={showUpdateModal}
        handleHide={() => setShowUpdateModal(false)}
        postUpdateAction={postUpdatePetAction}
        setLoginState={setLoginState}
      />
      <Row>
        <Form onSubmit={search}>
          <Row className="align-items-center">
            <Col lg={4}>
              <Form.Control
                name="searchPet"
                id="searchPet"
                placeholder="Search pets by name and breed"
              />
            </Col>
            <Col lg={6}>
              <Button type="submit">Search</Button>
            </Col>
            <Col lg={2}>
              <Button onClick={() => setShowAddModal(true)}>Add Pet</Button>
            </Col>
          </Row>
        </Form>
      </Row>
      <br />
      {errMsg !== undefined && <Alert variant="danger">{errMsg}</Alert>}
      <Row>
        <Table responsive striped bordered>
          <thead>
            <tr>
              <th>Id</th>
              <th>Name</th>
              {userType == "Employee" && <th>Owner</th>}
              <th>Weight</th>
              <th>Breed</th>
              <th colSpan={2}></th>
            </tr>
          </thead>
          <tbody>
            {pets.length != 0 &&
              pets.map((pet) => {
                return (
                  <tr key={pet.id}>
                    <td>{pet.id}</td>
                    <td>{pet.name}</td>
                    {userType == "Employee" && (
                      <td>
                        {pet.ownerFirstName} {pet.ownerLastName}
                      </td>
                    )}
                    <td>{pet.weight} lbs</td>
                    <td>{pet.breedName}</td>
                    <td>
                      <OverlayTrigger
                        placement="top"
                        overlay={<Tooltip>Update</Tooltip>}
                      >
                        <Button onClick={() => loadUpdateModal(pet)}>
                          <svg
                            xmlns="http://www.w3.org/2000/svg"
                            width="16"
                            height="16"
                            fill="currentColor"
                            className="bi bi-pencil-fill"
                            viewBox="0 0 16 16"
                          >
                            <path d="M12.854.146a.5.5 0 0 0-.707 0L10.5 1.793 14.207 5.5l1.647-1.646a.5.5 0 0 0 0-.708l-3-3zm.646 6.061L9.793 2.5 3.293 9H3.5a.5.5 0 0 1 .5.5v.5h.5a.5.5 0 0 1 .5.5v.5h.5a.5.5 0 0 1 .5.5v.5h.5a.5.5 0 0 1 .5.5v.207l6.5-6.5zm-7.468 7.468A.5.5 0 0 1 6 13.5V13h-.5a.5.5 0 0 1-.5-.5V12h-.5a.5.5 0 0 1-.5-.5V11h-.5a.5.5 0 0 1-.5-.5V10h-.5a.499.499 0 0 1-.175-.032l-.179.178a.5.5 0 0 0-.11.168l-2 5a.5.5 0 0 0 .65.65l5-2a.5.5 0 0 0 .168-.11l.178-.178z" />
                          </svg>
                        </Button>
                      </OverlayTrigger>
                    </td>
                    <td>
                      <OverlayTrigger
                        placement="top"
                        overlay={<Tooltip>Delete</Tooltip>}
                      >
                        <Button>
                          <svg
                            xmlns="http://www.w3.org/2000/svg"
                            width="16"
                            height="16"
                            fill="currentColor"
                            className="bi bi-trash3"
                            viewBox="0 0 16 16"
                          >
                            <path d="M6.5 1h3a.5.5 0 0 1 .5.5v1H6v-1a.5.5 0 0 1 .5-.5ZM11 2.5v-1A1.5 1.5 0 0 0 9.5 0h-3A1.5 1.5 0 0 0 5 1.5v1H2.506a.58.58 0 0 0-.01 0H1.5a.5.5 0 0 0 0 1h.538l.853 10.66A2 2 0 0 0 4.885 16h6.23a2 2 0 0 0 1.994-1.84l.853-10.66h.538a.5.5 0 0 0 0-1h-.995a.59.59 0 0 0-.01 0H11Zm1.958 1-.846 10.58a1 1 0 0 1-.997.92h-6.23a1 1 0 0 1-.997-.92L3.042 3.5h9.916Zm-7.487 1a.5.5 0 0 1 .528.47l.5 8.5a.5.5 0 0 1-.998.06L5 5.03a.5.5 0 0 1 .47-.53Zm5.058 0a.5.5 0 0 1 .47.53l-.5 8.5a.5.5 0 1 1-.998-.06l.5-8.5a.5.5 0 0 1 .528-.47ZM8 4.5a.5.5 0 0 1 .5.5v8.5a.5.5 0 0 1-1 0V5a.5.5 0 0 1 .5-.5Z" />
                          </svg>
                        </Button>
                      </OverlayTrigger>
                    </td>
                  </tr>
                );
              })}
            {pets.length == 0 && (
              <tr>
                <td colSpan={7} style={{ textAlign: "center" }}>
                  No pet data available. Please add a pet.
                </td>
              </tr>
            )}
          </tbody>
        </Table>
      </Row>

      <Pagination>
        {currPage != 1 && (
          <Pagination.Prev onClick={() => setCurrPage(currPage - 1)} />
        )}
        {currPage != totalPages && (
          <Pagination.Next onClick={() => setCurrPage(currPage + 1)} />
        )}
      </Pagination>
    </>
  );
}

export default PetManagement;
