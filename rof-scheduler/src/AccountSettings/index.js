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

import { ensureUpdateInformationProvided } from "../SharedServices/inputValidationService";

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
  const [validationMap, setValidationMap] = useState(new Map());
  const [displaySuccess, setDisplaySuccess] = useState(false);

  useEffect(() => {
    (async function () {
      try {
        const emp = await getEmployeeById();
        setEmployee(emp);
      } catch (e) {
        setUpdateErrMsg(e.message);                
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
    const email = submitEvent.target.email.value;
    const phoneNumber = submitEvent.target.phone.value;
    const addressLine1 = submitEvent.target.address1.value;
    const addressLine2 = submitEvent.target.address2.value;
    const city = submitEvent.target.city.value;
    const state = submitEvent.target.state.value;
    const zipCode = submitEvent.target.zip.value;

    const validationRes = ensureUpdateInformationProvided(
      firstName,
      lastName,
      ssn,
      role,
      username,
      email,
      phoneNumber,
      addressLine1,
      city,
      state,
      zipCode
    );
    
    setDisplaySuccess(false);

    if (validationRes.size > 0) {
      setValidationMap(validationRes);
    } else {
      setValidationMap(new Map());
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
            email,
            phoneNumber,
            addressLine1,
            addressLine2,
            city,
            state,
            zipCode
          );

          setEmployee({
            ...employee,
            firstName,
            lastName,
            ssn,
            role,
            username,
            email,
            phoneNumber,
            addressLine1,
            addressLine2,
            city,
            state,
            zipCode,
          });
          setUpdateErrMsg("");
          setDisplaySuccess(true);
        } catch (e) {
          setUpdateErrMsg(e.message);
        } finally {
          setLoading(false);
        }
      })();
    }
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
                  <Nav.Link eventKey="accountSettings" style={{"cursor":"pointer"}}>Account</Nav.Link>
                </Nav.Item>
                <Nav.Item>
                  <Nav.Link eventKey="passwordSettings" style={{"cursor":"pointer"}}>Password</Nav.Link>
                </Nav.Item>
              </Nav>
            </Col>

            <Col sm={9}>
              <Tab.Content>
                <Tab.Pane eventKey="accountSettings">
                  <Form noValidate onSubmit={handleSubmit}>
                    {updateErrMsg !== "" && <Alert variant="danger">{updateErrMsg}</Alert>}
                    {displaySuccess && <Alert variant="success">Successfully Updated!</Alert>}

                    <Row className="mb-3">
                      <Form.Group as={Col} md="4">
                        <Form.Label>First name</Form.Label>
                        <Form.Control
                          required
                          name="firstName"
                          type="text"
                          placeholder="First name"
                          defaultValue={employee.firstName}
                          isInvalid={validationMap.has("firstName")}
                        />
                        <Form.Control.Feedback type="invalid">
                          {validationMap.get("firstName")}
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
                          isInvalid={validationMap.has("lastName")}
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
                          isInvalid={validationMap.has("username")}
                        />
                        <Form.Control.Feedback type="invalid">
                          Please enter a username.
                        </Form.Control.Feedback>
                      </Form.Group>
                    </Row>

                    <Row className="mb-3">
                      <Form.Group as={Col} md="4">
                        <Form.Label>SSN</Form.Label>
                        <Form.Control
                          required
                          name="ssn"
                          type="text"
                          placeholder="SSN"
                          defaultValue={employee.ssn}
                          isInvalid={validationMap.has("ssn")}
                        />
                        <Form.Control.Feedback type="invalid">
                          {validationMap.get("ssn")}
                        </Form.Control.Feedback>
                      </Form.Group>

                      <Form.Group as={Col} md="4">
                        <Form.Label>Phone Number</Form.Label>
                        <Form.Control
                          required
                          name="phone"
                          type="text"
                          placeholder="Phone Number"
                          defaultValue={employee.phoneNumber}
                          isInvalid={validationMap.has("phone")}
                        />
                        <Form.Control.Feedback type="invalid">
                          {validationMap.get("phone")}
                        </Form.Control.Feedback>
                      </Form.Group>

                      <Form.Group as={Col} md="4">
                        <Form.Label>Email</Form.Label>
                        <Form.Control
                          required
                          name="email"
                          type="email"
                          placeholder="Email Address"
                          defaultValue={employee.emailAddress}
                          isInvalid={validationMap.has("email")}
                        />
                        <Form.Control.Feedback type="invalid">
                          {validationMap.get("email")}
                        </Form.Control.Feedback>
                      </Form.Group>                                      
                    </Row>

                    {/* <Row className="mb-3">
                      <Form.Group as={Col} md="4">
                        <Form.Label>Role</Form.Label>
                        <Form.Select
                          required
                          type="select"
                          placeholder="Role"
                          name="role"
                        >
                          <option value="Administrator">Administrator</option>
                          <option value="Employee">Employee</option>
                        </Form.Select>
                      </Form.Group>
                    </Row> */}

                    <Row className="mb-3">
                      <Form.Group as={Col} md="6">
                        <Form.Label>Address Line 1</Form.Label>
                        <Form.Control
                          required
                          name="address1"
                          type="text"
                          placeholder="Address Line 1"
                          defaultValue={employee.address.addressLine1}
                          isInvalid={validationMap.has("addressLine1")}
                        />
                        <Form.Control.Feedback type="invalid">
                          {validationMap.get("addressLine1")}
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
                      <Form.Group as={Col} md="3">
                        <Form.Label>City</Form.Label>
                        <Form.Control
                          required
                          name="city"
                          type="text"
                          placeholder="City"
                          defaultValue={employee.address.city}
                          isInvalid={validationMap.has("city")}
                        />
                        <Form.Control.Feedback type="invalid">
                          {validationMap.get("city")}
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
                          isInvalid={validationMap.has("state")}
                        />
                        <Form.Control.Feedback type="invalid">
                          {validationMap.get("state")}
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
                          isInvalid={validationMap.has("zipCode")}
                        />
                        <Form.Control.Feedback type="invalid">
                          {validationMap.get("zipcode")}
                        </Form.Control.Feedback>
                      </Form.Group>
                      
                      <Form.Group as={Col} md="3">
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
                        variant="primary"
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
                      variant="primary"
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
