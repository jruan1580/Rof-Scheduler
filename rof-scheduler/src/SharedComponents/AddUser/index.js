import { Modal, Button, Row, Form, Col } from 'react-bootstrap';

import { ensureCreateEmployeeInformationProvided } from "../../SharedServices/inputValidationService";

import { useState } from "react";

function AddUserModal({userType, show, handleHide}){
    const [validationMap, setValidationMap] = useState(new Map());

    const handleCreateUser = function(e){
        e.preventDefault();

        var firstName = e.target.firstName.value;
        var lastName = e.target.lastName.value;
        var ssn = e.target.ssn.value;
        var role = e.target.role.value;
        var username = e.target.username.value;
        var email = e.target.email.value;
        var phoneNumber = e.target.phoneNumber.value;
        var addressLine1 = e.target.addressLine1.value;
        var addressLine2 = e.target.addressLine2.value;
        var city = e.target.city.value;
        var state = e.target.state.value;
        var zipCode = e.target.zipCode.value;
        var password = e.target.password.value;
        var retypedPassword = e.target.retypedPassword.value;
        var inputValidations = new Map();

        //validate employee
        if (userType === "Employee"){
            inputValidations = ensureCreateEmployeeInformationProvided(
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
                zipCode,
                password,
                retypedPassword
            );
        }else{
            //validate client
        }

        console.log(inputValidations);
        if (inputValidations.size > 0) {
            setValidationMap(inputValidations);
        }else{
            setValidationMap(new Map());
        }
    }

    return(
        <>
            <Modal
                show={show}
                onHide={handleHide}
                size="lg"
                aria-labelledby="contained-modal-title-vcenter"
                centered
            >
                <Modal.Header closeButton>
                    <Modal.Title id="contained-modal-title-vcenter">
                        Add {userType}
                    </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form onSubmit={handleCreateUser}>
                        <h3>General User Information</h3>
                        <br/>
                        <Row>                                                       
                            <Form.Group as={Col} md="4">
                                <Form.Label>First Name</Form.Label>
                                <Form.Control placeholder="First Name" name="firstName" isInvalid={validationMap.has("firstName")}/>
                                <Form.Control.Feedback type="invalid">
                                    {validationMap.get("firstName")}
                                </Form.Control.Feedback>
                            </Form.Group>
                            <Form.Group as={Col} md="4">
                                <Form.Label>Last Name</Form.Label>
                                <Form.Control placeholder="Last Name" name="lastName" isInvalid={validationMap.has("lastName")}/>
                                <Form.Control.Feedback type="invalid">
                                    {validationMap.get("lastName")}
                                </Form.Control.Feedback>
                            </Form.Group>
                            <Form.Group as={Col} md="4">
                                <Form.Label>Email</Form.Label>
                                <Form.Control type="email" placeholder="Email" name="email" isInvalid={validationMap.has("email")}/>
                                <Form.Control.Feedback type="invalid">
                                    {validationMap.get("email")}
                                </Form.Control.Feedback>
                            </Form.Group>
                        </Row><br/>
                        
                        <Row>
                            <Form.Group as={Col} md="4">
                                <Form.Label>SSN</Form.Label>
                                <Form.Control placeholder="SSN" name="ssn" isInvalid={validationMap.has("ssn")}/>
                                <Form.Control.Feedback type="invalid">
                                    {validationMap.get("ssn")}
                                </Form.Control.Feedback>
                            </Form.Group>
                            <Form.Group as={Col} md="4">
                                <Form.Label>Phone Number</Form.Label>
                                <Form.Control placeholder="Phone Number" name="phoneNumber" isInvalid={validationMap.has("phone")}/>
                                <Form.Control.Feedback type="invalid">
                                    {validationMap.get("phone")}
                                </Form.Control.Feedback>
                            </Form.Group>
                        </Row><br/>

                        <Row>
                            <Form.Group as={Col} md="6">
                                <Form.Label>Address Line 1</Form.Label>
                                <Form.Control placeholder="Address Line 1" name="addressLine1" isInvalid={validationMap.has("addressLine1")}/>
                                <Form.Control.Feedback type="invalid">
                                    {validationMap.get("addressLine1")}
                                </Form.Control.Feedback>
                            </Form.Group>
                            <Form.Group as={Col} md="6">
                                <Form.Label>Address Line 2</Form.Label>
                                <Form.Control placeholder="Address Line 2 (Optional)" name="addressLine2" isInvalid={validationMap.has("addressLine2")}/>
                            </Form.Group>
                        </Row><br/>

                        <Row>
                            <Form.Group as={Col} md="3">
                                <Form.Label>City</Form.Label>
                                <Form.Control placeholder="City" name="city" isInvalid={validationMap.has("city")}/>
                                <Form.Control.Feedback type="invalid">
                                    {validationMap.get("city")}
                                </Form.Control.Feedback>
                            </Form.Group>
                            <Form.Group as={Col} md="3">
                                <Form.Label>State</Form.Label>
                                <Form.Control placeholder="State" name="state" isInvalid={validationMap.has("state")}/>
                                <Form.Control.Feedback type="invalid">
                                    {validationMap.get("state")}
                                </Form.Control.Feedback>
                            </Form.Group>
                            <Form.Group as={Col} md="3">
                                <Form.Label>Zip Code</Form.Label>
                                <Form.Control placeholder="Zip Code" name="zipCode" isInvalid={validationMap.has("zipCode")}/>
                                <Form.Control.Feedback type="invalid">
                                    {validationMap.get("zipCode")}
                                </Form.Control.Feedback>
                            </Form.Group>
                            <Form.Group as={Col} md="3">
                                <Form.Label>Country</Form.Label>
                                <Form.Control placeholder="Country" name="country" value="USA" disabled/>
                            </Form.Group>
                        </Row><br/>

                        <h2>Account Information</h2><br/>
                        <Row>
                            <Form.Group as={Col} md="5">
                                <Form.Label>Username</Form.Label>
                                <Form.Control placeholder="Username" name="username" isInvalid={validationMap.has("username")}/>
                                <Form.Control.Feedback type="invalid">
                                    {validationMap.get("username")}
                                </Form.Control.Feedback>
                            </Form.Group>
                            <Form.Group as={Col} md="5">
                                <Form.Label>Role</Form.Label>
                                <Form.Control placeholder="Role" name="role" isInvalid={validationMap.has("role")}/>
                                <Form.Control.Feedback type="invalid">
                                    {validationMap.get("role")}
                                </Form.Control.Feedback>
                            </Form.Group>
                        </Row><br/>

                        <Row>
                            <Form.Group as={Col} md="5">
                                <Form.Label>Temp Password</Form.Label>
                                <Form.Control type="password" placeholder="Password" name="password" isInvalid={validationMap.has("password")}/>
                                <Form.Control.Feedback type="invalid">
                                    {validationMap.get("password")}
                                </Form.Control.Feedback>
                            </Form.Group>

                            <Form.Group as={Col} md="5">
                                <Form.Label>Retype Temp Password</Form.Label>
                                <Form.Control type="password" placeholder="Retype Temp Password" name="retypedPassword" isInvalid={validationMap.has("retypedPassword")}/>
                                <Form.Control.Feedback type="invalid">
                                    {validationMap.get("retypedPassword")}
                                </Form.Control.Feedback>
                            </Form.Group>
                        </Row><br/>
                        <hr></hr>
                        
                        <Button type="button" variant='danger' onClick={() => handleHide()} className="float-end ms-2" >Cancel</Button>
                        <Button type="submit" className="float-end">Create</Button>
                        
                    </Form>
                </Modal.Body>
              
            </Modal>
        </>
    );
}

export default AddUserModal;