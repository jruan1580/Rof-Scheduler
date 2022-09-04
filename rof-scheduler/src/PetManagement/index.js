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
import "./index.css";
import {
  getAllPets,
  getPetsByClientId,
} from "../SharedServices/petManagementService";

function PetManagement({ setLoginState }) {
  const [userType, setUserType] = useState("");
  const [pets, setPets] = useState([]);
  const [errMsg, setErrMsg] = useState(undefined);
  const [currPage, setCurrPage] = useState(1);
  const [keyword, setKeyword] = useState("");
  const [totalPages, setTotalPages] = useState(0);
  const [showAddModal, setShowAddModal] = useState(false);

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
      searchEvent.target.searchClient.value === undefined
        ? ""
        : searchEvent.target.searchClient.value;
    setKeyword(searchTerm);
    setCurrPage(1);
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
              <th>Breed</th>
              <th>Pet Type</th>
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
                    <td>{pet.breedName}</td>
                    <td>{pet.petTypeName}</td>
                    {
                      <td>
                        <OverlayTrigger
                          placement="top"
                          overlay={<Tooltip>Update</Tooltip>}
                        >
                          <Button>
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
                    }
                    {/* delete button */}
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
