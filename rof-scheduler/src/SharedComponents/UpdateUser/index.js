import { Modal, Row, Form, Col, Button, Spinner, Alert } from "react-bootstrap";
import "./updateUser.css";
import { useState } from "react";

import { ensureUpdateInformationProvided } from "../../SharedServices/inputValidationService";
import { updateEmployeeInformation } from '../../SharedServices/employeeManagementService';

function UpdateUserModal({userType, user, show, hideModal, postUpdateAction}){
    const [validationMap, setValidationMap] = useState(new Map());
    const [updating, setUpdating] = useState(false);
    const [errMsg, setErrMsg] = useState(undefined);
    const [successMsg, setSuccessMsg] = useState(false);

    const closeModal = function(){
        setValidationMap(new Map());
        setErrMsg(undefined);
        setSuccessMsg(false);
        hideModal();
    }

    const handleUpdate = (e) =>{
        e.preventDefault();

        setErrMsg(undefined);
        setSuccessMsg(false);

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
       
        var inputValidations = new Map();

        if (userType === "Employee"){
            inputValidations = ensureUpdateInformationProvided(
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
        }else{
            //client
        }

        if (inputValidations.size > 0){
            setValidationMap(inputValidations);
        }else{
            setValidationMap(new Map());
            setUpdating(true);

            (async function(){
                try{
                    var updatedFields = new Map();
                    updatedFields.set('id', user.id);
                    
                    if (userType === "Employee"){
                        await updateEmployeeInformation(
                            user.id,
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

                        updatedFields.set('firstName', firstName);
                        updatedFields.set('lastName', lastName);
                        updatedFields.set('ssn', ssn);
                        updatedFields.set('role', role);
                        updatedFields.set('username', username);
                        updatedFields.set('email', email);
                        updatedFields.set('phoneNumber', phoneNumber);
                        updatedFields.set('address', { addressLine1, addressLine2, city, state, zipCode});
                    }else{

                    }

                    postUpdateAction(updatedFields);
                    setSuccessMsg(true);
                }catch(e){
                    setErrMsg(e.message);
                }finally{
                    setUpdating(false);
                }
            })();
        }
    }

    return(
        <>
             <Modal
                show={show}
                onHide={closeModal}
                dialogClassName="update-modal70"
                centered
            >
                <Modal.Header className='update-modal-header-color'>
                    <Modal.Title id="contained-modal-title-vcenter">
                        Update {userType}
                    </Modal.Title>
                </Modal.Header>

                <Modal.Body>
                    <Form onSubmit={handleUpdate}>
                        {errMsg !== undefined && <Alert variant="danger">{errMsg}</Alert>}
                        {successMsg && <Alert variant="success">{userType} successfully updated.</Alert>}
                        <h4>General User Information</h4>
                        <br/>
                        <Row>                                                       
                            <Form.Group as={Col} md="4">
                                    <Form.Label>First Name</Form.Label>
                                    <Form.Control placeholder="First Name" name="firstName" defaultValue={user === undefined ? "" : user.firstName} isInvalid={validationMap.has("firstName")}/>
                                    <Form.Control.Feedback type="invalid">
                                        {validationMap.get("firstName")}
                                    </Form.Control.Feedback>
                                </Form.Group>
                                <Form.Group as={Col} md="4">
                                    <Form.Label>Last Name</Form.Label>
                                    <Form.Control placeholder="Last Name" name="lastName" defaultValue={user === undefined ? "" : user.lastName} isInvalid={validationMap.has("lastName")}/>
                                    <Form.Control.Feedback type="invalid">
                                        {validationMap.get("lastName")}
                                    </Form.Control.Feedback>
                                </Form.Group>
                                <Form.Group as={Col} md="4">
                                    <Form.Label>Email</Form.Label>
                                    <Form.Control type="email" placeholder="Email" name="email" defaultValue={user === undefined ? "" : user.emailAddress} isInvalid={validationMap.has("email")}/>
                                    <Form.Control.Feedback type="invalid">
                                        {validationMap.get("email")}
                                    </Form.Control.Feedback>
                                </Form.Group>
                        </Row><br/>

                        <Row>
                            <Form.Group as={Col} md="4">
                                <Form.Label>SSN</Form.Label>
                                <Form.Control placeholder="SSN" name="ssn" defaultValue={user === undefined ? "" : user.ssn} isInvalid={validationMap.has("ssn")}/>
                                <Form.Control.Feedback type="invalid">
                                    {validationMap.get("ssn")}
                                </Form.Control.Feedback>
                            </Form.Group>
                            <Form.Group as={Col} md="4">
                                <Form.Label>Phone Number</Form.Label>
                                <Form.Control placeholder="Phone Number"  name="phoneNumber" defaultValue={user === undefined ? "" : user.phoneNumber} isInvalid={validationMap.has("phone")}/>
                                <Form.Control.Feedback type="invalid">
                                    {validationMap.get("phone")}
                                </Form.Control.Feedback>
                            </Form.Group>
                        </Row><br/>

                        <Row>
                            <Form.Group as={Col} md="6">
                                <Form.Label>Address Line 1</Form.Label>
                                <Form.Control 
                                    placeholder="Address Line 1" 
                                    name="addressLine1" 
                                    defaultValue={user === undefined || user.address === undefined || user.address === null ? "" : user.address.addressLine1} 
                                    isInvalid={validationMap.has("addressLine1")}
                                />
                                <Form.Control.Feedback type="invalid">
                                    {validationMap.get("addressLine1")}
                                </Form.Control.Feedback>
                            </Form.Group>
                            <Form.Group as={Col} md="6">
                                <Form.Label>Address Line 2</Form.Label>
                                <Form.Control 
                                    placeholder="Address Line 2 (Optional)" 
                                    name="addressLine2" 
                                    defaultValue={user === undefined || user.address === undefined || user.address === null ? "" : user.address.addressLine2} 
                                    isInvalid={validationMap.has("addressLine2")}
                                />
                            </Form.Group>
                        </Row><br/>

                        <Row>
                            <Form.Group as={Col} md="3">
                                <Form.Label>City</Form.Label>
                                <Form.Control 
                                    placeholder="City"
                                    name="city" 
                                    defaultValue={user === undefined || user.address === undefined || user.address === null ? "" : user.address.city} 
                                    isInvalid={validationMap.has("city")}
                                />
                                <Form.Control.Feedback type="invalid">
                                    {validationMap.get("city")}
                                </Form.Control.Feedback>
                            </Form.Group>
                            <Form.Group as={Col} md="3">
                                <Form.Label>State</Form.Label>
                                <Form.Control 
                                    placeholder="State" 
                                    name="state" 
                                    defaultValue={user === undefined || user.address === undefined || user.address === null ? "" : user.address.state} 
                                    isInvalid={validationMap.has("state")}
                                />
                                <Form.Control.Feedback type="invalid">
                                    {validationMap.get("state")}
                                </Form.Control.Feedback>
                            </Form.Group>
                            <Form.Group as={Col} md="3">
                                <Form.Label>Zip Code</Form.Label>
                                <Form.Control 
                                    placeholder="Zip Code" 
                                    name="zipCode" 
                                    defaultValue={user === undefined || user.address === undefined || user.address === null ? "" : user.address.zipCode} 
                                    isInvalid={validationMap.has("zipCode")}
                                />
                                <Form.Control.Feedback type="invalid">
                                    {validationMap.get("zipCode")}
                                </Form.Control.Feedback>
                            </Form.Group>
                            <Form.Group as={Col} md="3">
                                <Form.Label>Country</Form.Label>
                                <Form.Control placeholder="Country" name="country" value="USA" disabled/>
                            </Form.Group>
                        </Row><br/>
                        <h4>Account Information</h4><br/>
                        <Row>
                            <Form.Group as={Col} md="3">
                                <Form.Label>Username</Form.Label>
                                <Form.Control placeholder="Username" name="username" defaultValue={user === undefined ? "" : user.username} isInvalid={validationMap.has("username")}/>
                                <Form.Control.Feedback type="invalid">
                                    {validationMap.get("username")}
                                </Form.Control.Feedback>
                            </Form.Group>
                            <Form.Group as={Col} md="3">
                                <Form.Label>Role</Form.Label>
                                    <Form.Select type="select" placeholder="Role" name="role" defaultValue={user === undefined ? "" : user.role} isInvalid={validationMap.has("role")}>
                                        <option value="Administrator">Administrator</option>
                                        <option value="Employee">Employee</option>
                                    </Form.Select>
                                <Form.Control.Feedback type="invalid">
                                    {validationMap.get("role")}
                                </Form.Control.Feedback>
                            </Form.Group>
                        </Row>
                        <hr></hr>                       
                        {
                           (updating) &&
                           <Button type="button" variant='danger' className="float-end ms-2" disabled>Cancel</Button>
                        }
                        {
                            (!updating) &&
                            <Button type="button" variant='danger' onClick={() => closeModal()} className="float-end ms-2">Cancel</Button>
                        }                        
                        {
                            (updating) && (
                            <Button variant="primary" className="float-end" disabled>
                            <Spinner
                                as="span"
                                animation="grow"
                                size="sm"
                                role="status"
                                aria-hidden="true"
                            />
                            Updating...
                            </Button>
                        )}
                        {
                            (!updating) &&
                            <Button type="submit" className="float-end">Update</Button>
                        }
                    </Form>
                </Modal.Body>
            </Modal>
        </>
    )
}

export default UpdateUserModal;