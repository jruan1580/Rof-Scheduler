import { Row, Col, Container, Form, Button } from "react-bootstrap";

function CreateClient() {
  return (
    <>
      <Container
        style={{
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          paddingTop: "2rem",
        }}
      >
        <Form>
          <h4>General Information</h4>
          <Row className="mb-3">
            <Form.Group as={Col} md="6">
              <Form.Label>First name</Form.Label>
              <Form.Control
                required
                name="firstName"
                type="text"
                placeholder="First name"
                // defaultValue={employee.firstName}
                // isInvalid={validationMap.has("firstName")}
              />
              {/* <Form.Control.Feedback type="invalid">
                {validationMap.get("firstName")}
              </Form.Control.Feedback> */}
            </Form.Group>

            <Form.Group as={Col} md="6">
              <Form.Label>Last name</Form.Label>
              <Form.Control
                required
                name="lastName"
                type="text"
                placeholder="Last name"
                // defaultValue={employee.lastName}
                // isInvalid={validationMap.has("lastName")}
              />
              {/* <Form.Control.Feedback type="invalid">
                Please enter your last name.
              </Form.Control.Feedback> */}
            </Form.Group>
          </Row>

          <Row className="mb-3">
            <Form.Group as={Col} md="6">
              <Form.Label>Username</Form.Label>
              <Form.Control
                required
                type="text"
                placeholder="Username"
                name="username"
                // defaultValue={employee.username}
                // isInvalid={validationMap.has("username")}
              />
              {/* <Form.Control.Feedback type="invalid">
                Please enter a username.
              </Form.Control.Feedback> */}
            </Form.Group>
            <Form.Group as={Col} md="6">
              <Form.Label>Password</Form.Label>
              <Form.Control
                required
                type="text"
                placeholder="Password"
                name="password"
                // defaultValue={employee.username}
                // isInvalid={validationMap.has("username")}
              />
              {/* <Form.Control.Feedback type="invalid">
                Please enter a username.
              </Form.Control.Feedback> */}
            </Form.Group>
          </Row>

          <br />
          <hr />
          <h4>Contact Information</h4>
          <Row className="mb-3">
            <Form.Group as={Col} md="6">
              <Form.Label>Address Line 1</Form.Label>
              <Form.Control
                required
                name="address1"
                type="text"
                placeholder="Address Line 1"
                // defaultValue={employee.address.addressLine1}
                // isInvalid={validationMap.has("addressLine1")}
              />
              {/* <Form.Control.Feedback type="invalid">
                {validationMap.get("addressLine1")}
              </Form.Control.Feedback> */}
            </Form.Group>
            <Form.Group as={Col} md="6">
              <Form.Label>Address Line 2</Form.Label>
              <Form.Control
                type="text"
                name="address2"
                placeholder="Address Line 2"
                // defaultValue={employee.address.addressLine2}
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
                // defaultValue={employee.address.city}
                // isInvalid={validationMap.has("city")}
              />
              {/* <Form.Control.Feedback type="invalid">
                {validationMap.get("city")}
              </Form.Control.Feedback> */}
            </Form.Group>
            <Form.Group as={Col} md="3">
              <Form.Label>State</Form.Label>
              <Form.Control
                required
                name="state"
                type="text"
                placeholder="State"
                // defaultValue={employee.address.state}
                // isInvalid={validationMap.has("state")}
              />
              {/* <Form.Control.Feedback type="invalid">
                {validationMap.get("state")}
              </Form.Control.Feedback> */}
            </Form.Group>
            <Form.Group as={Col} md="3">
              <Form.Label>Zipcode</Form.Label>
              <Form.Control
                required
                name="zip"
                type="text"
                placeholder="Zipcode"
                // defaultValue={employee.address.zipCode}
                // isInvalid={validationMap.has("zipCode")}
              />
              {/* <Form.Control.Feedback type="invalid">
                {validationMap.get("zipcode")}
              </Form.Control.Feedback> */}
            </Form.Group>
          </Row>

          <Row className="mb-3">
            <Form.Group as={Col} md="6">
              <Form.Label>Country</Form.Label>
              <Form.Control
                required
                type="text"
                placeholder="Country"
                disabled
                defaultValue="USA"
              />
            </Form.Group>
            <Form.Group as={Col} md="6">
              <Form.Label>Email</Form.Label>
              <Form.Control required type="text" placeholder="Email" />
            </Form.Group>
          </Row>

          <Row className="mb-3">
            <Form.Group as={Col} md="6">
              <Form.Label>Primary Phone Number</Form.Label>
              <Form.Control
                required
                name="primPhone"
                type="text"
                placeholder="Primary Phone Number"
                // defaultValue={employee.address.city}
                // isInvalid={validationMap.has("city")}
              />
              {/* <Form.Control.Feedback type="invalid">
                {validationMap.get("city")}
              </Form.Control.Feedback> */}
            </Form.Group>
            <Form.Group as={Col} md="6">
              <Form.Label>Secondary Phone Number</Form.Label>
              <Form.Control
                required
                name="secPhone"
                type="text"
                placeholder="Secondary Phone Number"
                // defaultValue={employee.address.state}
                // isInvalid={validationMap.has("state")}
              />
              {/* <Form.Control.Feedback type="invalid">
                {validationMap.get("state")}
              </Form.Control.Feedback> */}
            </Form.Group>
          </Row>
          <br />
          <Button variant="primary" type="submit">
            Submit
          </Button>
        </Form>
      </Container>
    </>
  );
}

export default CreateClient;
