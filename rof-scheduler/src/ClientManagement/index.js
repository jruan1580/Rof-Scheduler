import { useEffect, useState } from "react";
import { Form, Button, Row, Col, Alert, Pagination } from "react-bootstrap";
import AddUserModal from "../SharedComponents/AddUser";
import GenericUserTable from "../SharedComponents/UserTable";
import UpdateUserModal from "../SharedComponents/UpdateUser";
import LoadingModal from "../SharedComponents/LoadingModal";
import {
  getAllClients,
  resetClientLockStatus,
} from "../SharedServices/clientManagementService";

function ClientManagement({setLoginState}) {
  const [clients, setClients] = useState([]);
  const [currPage, setCurrPage] = useState(1);
  const [errorMessage, setErrorMessage] = useState(undefined);
  const [keyword, setKeyword] = useState("");
  const [totalPages, setTotalPages] = useState(0);
  const [showAddModal, setShowAddModal] = useState(false);
  const [showUpdateModal, setShowUpdateModal] = useState(false);
  const [currClientToUpdate, setCurrClientToUpdate] = useState(undefined);
  const [showLoadingModal, setShowLoadingModal] = useState(false);

  useEffect(() => {
    (async function () {
      try {
        var resp = await getAllClients(currPage, 10, keyword);

        if (resp.status === 401){
          setLoginState(false);
          return;
        }

        const clientByPage = await resp.json();

        setClients(clientByPage.clients);
        setTotalPages(clientByPage.totalPages);
      } catch (e) {
        setErrorMessage(e.message);
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

  const reloadAfterThreeSeconds = () => {
    setTimeout(() => window.location.reload(), 3000);
  };

  const resetClientLock = (id) => {
    (async function () {
      try {
        setShowLoadingModal(true);
        var resp = await resetClientLockStatus(id);
        if (resp.status === 401){
          setLoginState(false);
          return;
        }

        for (var i = 0; i < clients.length; i++) {
          if (clients[i].id === id) {
            clients[i].isLocked = false;
            break;
          }
        }

        setClients(clients);
      } catch (e) {
        setErrorMessage(e.message);
      } finally {
        setShowLoadingModal(false);
      }
    })();
  };

  const postUpdateClientAction = (updatedFieldsMap) => {
    for (var i = 0; i < clients.length; i++) {
      if (clients[i].id === updatedFieldsMap.get("id")) {
        clients[i].fullName =
          updatedFieldsMap.get("firstName") +
          " " +
          updatedFieldsMap.get("lastName");
        clients[i].firstName = updatedFieldsMap.get("firstName");
        clients[i].lastName = updatedFieldsMap.get("lastName");
        clients[i].emailAddress = updatedFieldsMap.get("email");
        clients[i].username = updatedFieldsMap.get("username");
        clients[i].primaryPhoneNum = updatedFieldsMap.get("phoneNumber");
        clients[i].secPhoneNum = updatedFieldsMap.get("secPhoneNum");
        clients[i].address = updatedFieldsMap.get("address");

        break;
      }
    }

    setClients(clients);
  };

  const loadUpdateModal = (user) => {
    setCurrClientToUpdate(user);

    setShowUpdateModal(true);
  };

  const closeUpdateModal = () => {
    setCurrClientToUpdate(undefined);

    setShowUpdateModal(false);
  };

  return (
    <>
      <h1>Client Management</h1>
      <br />
      <AddUserModal
        userType="Client"
        show={showAddModal}
        handleHide={() => setShowAddModal(false)}
        handleUserAddSuccess={reloadAfterThreeSeconds}
      />
      <UpdateUserModal
        user={currClientToUpdate}
        userType="Client"
        show={showUpdateModal}
        hideModal={closeUpdateModal}
        postUpdateAction={postUpdateClientAction}
      />
      <LoadingModal show={showLoadingModal} />
      <Row>
        <Form onSubmit={search}>
          <Row className="align-items-center">
            <Col lg={4}>
              <Form.Control
                name="searchClient"
                id="searchClient"
                placeholder="Search client by name, email..etc."
              />
            </Col>
            <Col lg={6}>
              <Button type="submit">Search</Button>
            </Col>
            <Col lg={2}>
              <Button onClick={() => setShowAddModal(true)}>Add Client</Button>
            </Col>
          </Row>
        </Form>
      </Row>
      <br />
      {errorMessage !== undefined && (
        <Alert variant="danger">{errorMessage}</Alert>
      )}
      <Row>
        <GenericUserTable
          userType="Client"
          users={clients}
          resetUserLockStatus={resetClientLock}
          showUpdateModal={loadUpdateModal}
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

export default ClientManagement;
