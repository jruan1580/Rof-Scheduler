import { Tab, Nav, Row, Col, Form, Button, Container } from "react-bootstrap";
import { getEmployeeById } from "../SharedServices/employeeManagementService";
import { useEffect, useState } from "react";

function AccountSettings() {
  const [employee, setEmployee] = useState({
    firstName: "",
    lastName: "",
    ssn: "",
    role: "",
    userName: "",
    address1: "",
    address2: "",
    city: "",
    state: "",
    zip: "",
  });

  // useEffect(() => {
  //   (async function () {
  //     try {
  //       const emp = await getEmployeeById();
  //       setEmployee(emp);
  //     } catch (e) {
  //       alert("Failed to fetch employee info... Try Again");
  //       console.log(e.message);
  //     }
  //   })();
  // }, []);

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
                  <Form>
                    <Row className="mb-3">
                      <Form.Group as={Col} md="4">
                        <Form.Label>First name</Form.Label>
                        <Form.Control
                          required
                          type="text"
                          placeholder="First name"
                          defaultValue={employee.firstName}
                        />
                      </Form.Group>

                      <Form.Group as={Col} md="4">
                        <Form.Label>Last name</Form.Label>
                        <Form.Control
                          required
                          type="text"
                          placeholder="Last name"
                          defaultValue={employee.lastName}
                        />
                      </Form.Group>

                      <Form.Group as={Col} md="4">
                        <Form.Label>Username</Form.Label>
                        <Form.Control
                          required
                          type="text"
                          placeholder="Username"
                          disabled
                          defaultValue={employee.userName}
                        />
                      </Form.Group>
                    </Row>

                    <Row className="mb-3">
                      <Form.Group as={Col} md="4">
                        <Form.Label>Role</Form.Label>
                        <Form.Select required type="select" placeholder="Role">
                          <option value="admin">Admin</option>
                          <option value="employee">Employee</option>
                        </Form.Select>
                      </Form.Group>

                      <Form.Group as={Col} md="4">
                        <Form.Label>SSN</Form.Label>
                        <Form.Control
                          required
                          type="text"
                          placeholder="SSN"
                          defaultValue={employee.ssn}
                        />
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
                          type="text"
                          placeholder="Address Line 1"
                          defaultValue={employee.address1}
                        />
                      </Form.Group>
                      <Form.Group as={Col} md="6">
                        <Form.Label>Address Line 2</Form.Label>
                        <Form.Control
                          type="text"
                          placeholder="Address Line 2"
                          defaultValue={employee.address2}
                        />
                      </Form.Group>
                    </Row>

                    <Row className="mb-3">
                      <Form.Group as={Col} md="6">
                        <Form.Label>City</Form.Label>
                        <Form.Control
                          required
                          type="text"
                          placeholder="City"
                          defaultValue={employee.address2}
                        />
                      </Form.Group>
                      <Form.Group as={Col} md="3">
                        <Form.Label>State</Form.Label>
                        <Form.Control
                          required
                          type="text"
                          placeholder="State"
                          defaultValue={employee.state}
                        />
                      </Form.Group>
                      <Form.Group as={Col} md="3">
                        <Form.Label>Zipcode</Form.Label>
                        <Form.Control
                          required
                          type="text"
                          placeholder="Zipcode"
                          defaultValue={employee.zip}
                        />
                      </Form.Group>
                    </Row>

                    <Button
                      variant="outline-primary"
                      type="submit"
                      style={{ marginTop: "0.5rem" }}
                    >
                      Save
                    </Button>
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
