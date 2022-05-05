import {
  Tab,
  Nav,
  Row,
  Col,
  Form,
  Button,
  Container,
  Spinner,
  Alert,
} from "react-bootstrap";
import {
  getEmployeeById,
  updateEmployeeInformation,
} from "../SharedServices/employeeManagementService";
import { useEffect, useState } from "react";

function AccountSettings() {
  const ee = {
    id: 0,
    firstName: "",
    lastName: "",
    ssn: "",
    role: "",
    username: "",
    address: {
      addressLine1: "",
      addressLine2: "",
      city: "",
      state: "",
      zipCode: "",
    },
  };

  const [employee, setEmployee] = useState(ee);
  const [loading, setLoading] = useState(false);
  const [updateErrMsg, setUpdateErrMsg] = useState("");
  const [firstNameError, setFirstNameError] = useState(false);
  const [lastNameError, setlastNameError] = useState(false);
  const [usernameError, setUsernameError] = useState(false);
  const [ssnError, setSsnError] = useState(false);
  const [address1Error, setAddress1Error] = useState(false);
  const [cityError, setCityError] = useState(false);
  const [stateError, setStateError] = useState(false);
  const [zipError, setZipError] = useState(false);

  useEffect(() => {
    (async function () {
      try {
        const emp = await getEmployeeById();
        setEmployee(emp);
      } catch (e) {
        alert("Failed to fetch employee info... Try Again");
        console.log(e.message);
      }
    })();
  }, []);

  const handleSubmit = (submitEvent) => {
    submitEvent.preventDefault();

    const id = parseInt(localStorage.getItem("id"));
    const firstName = submitEvent.target.firstName.value;
    const lastName = submitEvent.target.lastName.value;
    const ssn = submitEvent.target.ssn.value;
    const role = submitEvent.target.role.value;
    const username = submitEvent.target.username.value;
    const addressLine1 = submitEvent.target.address1.value;
    const addressLine2 = submitEvent.target.address2.value;
    const city = submitEvent.target.city.value;
    const state = submitEvent.target.state.value;
    const zipCode = submitEvent.target.zip.value;

    if (firstName === undefined || firstName === "") {
      setFirstNameError(true);
      submitEvent.stopPropagation();
    } else {
      setFirstNameError(false);
    }

    if (lastName === undefined || lastName === "") {
      setlastNameError(true);
      submitEvent.stopPropagation();
    } else {
      setlastNameError(false);
    }

    if (ssn === undefined || ssn === "") {
      setSsnError(true);
      submitEvent.stopPropagation();
    } else {
      setSsnError(false);
    }

    if (username === undefined || username === "") {
      setUsernameError(true);
      submitEvent.stopPropagation();
    } else {
      setUsernameError(false);
    }
    if (addressLine1 === undefined || addressLine1 === "") {
      setAddress1Error(true);
      submitEvent.stopPropagation();
    } else {
      setAddress1Error(false);
    }

    if (city === undefined || city === "") {
      setCityError(true);
      submitEvent.stopPropagation();
    } else {
      setCityError(false);
    }

    if (state === undefined || state === "") {
      setStateError(true);
      submitEvent.stopPropagation();
    } else {
      setStateError(false);
    }

    if (zipCode === undefined || zipCode === "") {
      setZipError(true);
      submitEvent.stopPropagation();
    } else {
      setZipError(false);
    }

    setLoading(true);
    (async function () {
      try {
        await updateEmployeeInformation(
          id,
          firstName,
          lastName,
          ssn,
          role,
          username,
          addressLine1,
          addressLine2,
          city,
          state,
          zipCode
        );

        setEmployee({
          ...employee,
          firstName: submitEvent.target.firstName.value,
          lastName: submitEvent.target.lastName.value,
          ssn: submitEvent.target.ssn.value,
          role: submitEvent.target.role.value,
          username: submitEvent.target.username.value,
          addressLine1: submitEvent.target.address1,
          addressLine2: submitEvent.target.address2.value,
          city: submitEvent.target.city.value,
          state: submitEvent.target.state.value,
          zipCode: submitEvent.target.zip.value,
        });
        setUpdateErrMsg("");
      } catch (e) {
        setUpdateErrMsg(e.message);
      } finally {
        setLoading(false);
      }
    })();
  };

  return (
    <>
      <Container className="settingsContainer">
        <h2 style={{ paddingBottom: "1.5rem", fontWeight: "bold" }}>
          Account Settings
        </h2>

        <Tab.Container id="settings" defaultActiveKey="accountSettings">
          <Row>
            <Col sm={3}>
              <Nav variant="pills" className="flex-column">
                <Nav.Item>
                  <Nav.Link eventKey="accountSettings">Account</Nav.Link>
                </Nav.Item>
                <Nav.Item>
                  <Nav.Link eventKey="passwordSettings">Password</Nav.Link>
                </Nav.Item>
              </Nav>
            </Col>

            <Col sm={9}>
              <Tab.Content>
                <Tab.Pane eventKey="accountSettings">
                  <Form noValidate onSubmit={handleSubmit}>
                    {updateErrMsg !== "" && (
                      <Alert variant="danger">{updateErrMsg}</Alert>
                    )}

                    <Row className="mb-3">
                      <Form.Group as={Col} md="4">
                        <Form.Label>First name</Form.Label>
                        <Form.Control
                          required
                          name="firstName"
                          type="text"
                          placeholder="First name"
                          defaultValue={employee.firstName}
                          isInvalid={firstNameError}
                        />
                        <Form.Control.Feedback type="invalid">
                          Please enter your first name.
                        </Form.Control.Feedback>
                      </Form.Group>

                      <Form.Group as={Col} md="4">
                        <Form.Label>Last name</Form.Label>
                        <Form.Control
                          required
                          name="lastName"
                          type="text"
                          placeholder="Last name"
                          defaultValue={employee.lastName}
                          isInvalid={lastNameError}
                        />
                        <Form.Control.Feedback type="invalid">
                          Please enter your last name.
                        </Form.Control.Feedback>
                      </Form.Group>

                      <Form.Group as={Col} md="4">
                        <Form.Label>Username</Form.Label>
                        <Form.Control
                          required
                          type="text"
                          placeholder="Username"
                          name="username"
                          defaultValue={employee.username}
                          isInvalid={usernameError}
                        />
                        <Form.Control.Feedback type="invalid">
                          Please enter a username.
                        </Form.Control.Feedback>
                      </Form.Group>
                    </Row>

                    <Row className="mb-3">
                      <Form.Group as={Col} md="4">
                        <Form.Label>Role</Form.Label>
                        <Form.Select
                          required
                          type="select"
                          placeholder="Role"
                          name="role"
                          defaultValue={employee.role}
                        >
                          <option value="Administrator">Administrator</option>
                          <option value="Employee">Employee</option>
                        </Form.Select>
                      </Form.Group>

                      <Form.Group as={Col} md="4">
                        <Form.Label>SSN</Form.Label>
                        <Form.Control
                          required
                          name="ssn"
                          type="text"
                          placeholder="SSN"
                          defaultValue={employee.ssn}
                          isInvalid={ssnError}
                        />
                        <Form.Control.Feedback type="invalid">
                          Please enter your SSN.
                        </Form.Control.Feedback>
                      </Form.Group>

                      <Form.Group as={Col} md="4">
                        <Form.Label>Country</Form.Label>
                        <Form.Control
                          required
                          type="text"
                          placeholder="Country"
                          disabled
                          defaultValue="USA"
                        />
                      </Form.Group>
                    </Row>

                    <Row className="mb-3">
                      <Form.Group as={Col} md="6">
                        <Form.Label>Address Line 1</Form.Label>
                        <Form.Control
                          required
                          name="address1"
                          type="text"
                          placeholder="Address Line 1"
                          defaultValue={employee.address.addressLine1}
                          isInvalid={address1Error}
                        />
                        <Form.Control.Feedback type="invalid">
                          Please enter your address.
                        </Form.Control.Feedback>
                      </Form.Group>
                      <Form.Group as={Col} md="6">
                        <Form.Label>Address Line 2</Form.Label>
                        <Form.Control
                          type="text"
                          name="address2"
                          placeholder="Address Line 2"
                          defaultValue={employee.address.addressLine2}
                        />
                      </Form.Group>
                    </Row>

                    <Row className="mb-3">
                      <Form.Group as={Col} md="6">
                        <Form.Label>City</Form.Label>
                        <Form.Control
                          required
                          name="city"
                          type="text"
                          placeholder="City"
                          defaultValue={employee.address.city}
                          isInvalid={cityError}
                        />
                        <Form.Control.Feedback type="invalid">
                          Please enter your city.
                        </Form.Control.Feedback>
                      </Form.Group>
                      <Form.Group as={Col} md="3">
                        <Form.Label>State</Form.Label>
                        <Form.Control
                          required
                          name="state"
                          type="text"
                          placeholder="State"
                          defaultValue={employee.address.state}
                          isInvalid={stateError}
                        />
                        <Form.Control.Feedback type="invalid">
                          Please enter your state.
                        </Form.Control.Feedback>
                      </Form.Group>
                      <Form.Group as={Col} md="3">
                        <Form.Label>Zipcode</Form.Label>
                        <Form.Control
                          required
                          name="zip"
                          type="text"
                          placeholder="Zipcode"
                          defaultValue={employee.address.zipCode}
                          isInvalid={zipError}
                        />
                        <Form.Control.Feedback type="invalid">
                          Please enter your zipcode.
                        </Form.Control.Feedback>
                      </Form.Group>
                    </Row>

                    {loading && (
                      <Button variant="primary" disabled>
                        <Spinner
                          as="span"
                          animation="grow"
                          size="sm"
                          role="status"
                          aria-hidden="true"
                        />
                        Loading...
                      </Button>
                    )}
                    {!loading && (
                      <Button
                        variant="outline-primary"
                        type="submit"
                        style={{ marginTop: "0.5rem" }}
                      >
                        Save
                      </Button>
                    )}
                  </Form>
                </Tab.Pane>

                <Tab.Pane eventKey="passwordSettings">
                  <Form>
                    <Form.Group className="mb-3" controlId="oldPassword">
                      <Form.Label>Old Password</Form.Label>
                      <Form.Control type="password" placeholder="Password" />
                    </Form.Group>

                    <Form.Group className="mb-3" controlId="newPassword">
                      <Form.Label>New Password</Form.Label>
                      <Form.Control type="password" placeholder="Password" />
                    </Form.Group>

                    <Form.Group className="mb-3" controlId="confirm">
                      <Form.Label>Confirm Password</Form.Label>
                      <Form.Control type="password" placeholder="Password" />
                    </Form.Group>

                    <Button
                      variant="outline-primary"
                      type="submit"
                      style={{ marginTop: "0.5rem" }}
                    >
                      Change Password
                    </Button>
                  </Form>
                </Tab.Pane>
              </Tab.Content>
            </Col>
          </Row>
        </Tab.Container>
      </Container>
    </>
  );
}

export default AccountSettings;
