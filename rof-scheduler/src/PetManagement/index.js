import { useState } from "react";
import { Form, Button, Row, Col } from "react-bootstrap";
import PetTable from "./PetTable";
import AddPetModal from "./AddModal";

function PetManagement({ setLoginState }) {
  const [pets, setPets] = useState([]);
  const [currPage, setCurrPage] = useState(1);
  const [errorMessage, setErrorMessage] = useState(undefined);
  const [keyword, setKeyword] = useState("");
  const [totalPages, setTotalPages] = useState(0);
  const [showAddModal, setShowAddModal] = useState(false);

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
        <Form>
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
      <Row>
        <PetTable
          // userType=""
          pets={pets}
          // showUpdateModal={loadUpdateModal}
        />
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
