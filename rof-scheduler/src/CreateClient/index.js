import {
  Row,
  Col,
  Container,
  Form,
  Button,
  Alert,
  Spinner,
} from "react-bootstrap";
import { useState } from "react";
import { ensureCreateClientInformationProvided } from "../SharedServices/inputValidationService";
import { createClient } from "../SharedServices/clientManagementService";

function CreateClient() {
  const clnt = {
    countryId: 1,
    firstName: "",
    lastName: "",
    email: "",
    username: "",
    password: "",
    primaryPhoneNum: "",
    secondaryPhoneNum: "",
    address: {
      addressLine1: "",
      addressLine2: "",
      city: "",
      state: "",
      zipCode: "",
    },
  };

  const [client, setClient] = useState(clnt);
  const [loading, setLoading] = useState(false);
  const [errMsg, setErrMsg] = useState("");
  const [validationMap, setValidationMap] = useState(new Map());
  const [showPassword, setShowPassword] = useState(false);

  const togglePassword = () => {
    setShowPassword(!showPassword);
  };

  const handleSubmit = (submitEvent) => {
    submitEvent.preventDefault();

    const countryId = 1;
    const firstName = submitEvent.target.firstName.value;
    const lastName = submitEvent.target.lastName.value;
    const emailAddress = submitEvent.target.emailAdd.value;
    const username = submitEvent.target.username.value;
    const password = submitEvent.target.password.value;
    const addressLine1 = submitEvent.target.address1.value;
    const addressLine2 = submitEvent.target.address2.value;
    const city = submitEvent.target.city.value;
    const state = submitEvent.target.state.value;
    const zipCode = submitEvent.target.zip.value;
    const primaryPhoneNum = submitEvent.target.primPhone.value;
    const secondaryPhoneNum = submitEvent.target.secPhone.value;

    const validationRes = ensureCreateClientInformationProvided(
      firstName,
      lastName,
      emailAddress,
      username,
      password,
      primaryPhoneNum,
      addressLine1,
      city,
      state,
      zipCode
    );

    if (validationRes.size > 0) {
      setValidationMap(validationRes);
    } else {
      setValidationMap(new Map());
      setLoading(true);

      (async function () {
        try {
          await createClient(
            countryId,
            firstName,
            lastName,
            emailAddress,
            username,
            password,
            primaryPhoneNum,
            secondaryPhoneNum,
            addressLine1,
            addressLine2,
            city,
            state,
            zipCode
          );

          setClient({
            ...client,
            firstName: submitEvent.target.firstName.value,
            lastName: submitEvent.target.lastName.value,
            emailAddress: submitEvent.target.emailAdd.value,
            username: submitEvent.target.username.value,
            password: submitEvent.target.password.value,
            primaryPhoneNum: submitEvent.target.primPhone.value,
            secondaryPhoneNum: submitEvent.target.secPhone.value,
            addressLine1: submitEvent.target.address1.value,
            addressLine2: submitEvent.target.address2.value,
            city: submitEvent.target.city.value,
            state: submitEvent.target.state.value,
            zipCode: submitEvent.target.zip.value,
          });

          setErrMsg("");
        } catch (e) {
          setErrMsg(e.message);
        } finally {
          setLoading(false);
        }
      })();
    }
  };

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
        <Form noValidate onSubmit={handleSubmit}>
          {errMsg !== "" && <Alert variant="danger">{errMsg}</Alert>}

          <h4>General Information</h4>
          <Row className="mb-3">
            <Form.Group as={Col} md="6">
              <Form.Label>First name</Form.Label>
              <Form.Control
                required
                name="firstName"
                type="text"
                placeholder="First name"
                isInvalid={validationMap.has("firstName")}
              />
              <Form.Control.Feedback type="invalid">
                {validationMap.get("firstName")}
              </Form.Control.Feedback>
            </Form.Group>

            <Form.Group as={Col} md="6">
              <Form.Label>Last name</Form.Label>
              <Form.Control
                required
                name="lastName"
                type="text"
                placeholder="Last name"
                isInvalid={validationMap.has("lastName")}
              />
              <Form.Control.Feedback type="invalid">
                Please enter your last name.
              </Form.Control.Feedback>
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
                isInvalid={validationMap.has("username")}
              />
              <Form.Control.Feedback type="invalid">
                Please enter a username.
              </Form.Control.Feedback>
            </Form.Group>
            <Form.Group as={Col} md="6">
              <Form.Label>Password</Form.Label>
              <Form.Control
                required
                // type={showPassword ? "text" : "password"}
                type="password"
                placeholder="Password"
                name="password"
                isInvalid={validationMap.has("password")}
              />
              {/* <InputGroup.Append>
                <InputGroup.Text>
                  <i
                    onClick={togglePassword}
                    class={showPassword ? "fas fa-eye-slash" : "fas fa-eye"}
                  ></i>
                </InputGroup.Text>
              </InputGroup.Append> */}
              <Form.Control.Feedback type="invalid">
                Please enter a password.
              </Form.Control.Feedback>
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
                isInvalid={validationMap.has("zipCode")}
              />
              <Form.Control.Feedback type="invalid">
                {validationMap.get("zipCode")}
              </Form.Control.Feedback>
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
              <Form.Control
                required
                type="email"
                placeholder="Email"
                name="emailAdd"
                isInvalid={validationMap.has("email")}
              />
              <Form.Control.Feedback type="invalid">
                {validationMap.get("email")}
              </Form.Control.Feedback>
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
                isInvalid={validationMap.has("primaryPhone")}
              />
              <Form.Control.Feedback type="invalid">
                {validationMap.get("primaryPhone")}
              </Form.Control.Feedback>
            </Form.Group>
            <Form.Group as={Col} md="6">
              <Form.Label>Secondary Phone Number</Form.Label>
              <Form.Control
                required
                name="secPhone"
                type="text"
                placeholder="Secondary Phone Number"
              />
            </Form.Group>
          </Row>

          <br />

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
            <Button variant="primary" type="submit">
              Create
            </Button>
          )}
        </Form>
      </Container>
    </>
  );
}

export default CreateClient;
