import { Modal, Row, Form, Col, Alert } from "react-bootstrap";

import { useState } from "react";

import "../../SharedCSS/modal.css";

import UpdateEntityBtn from "../../SharedComponents/UpdateEntityBtn";

import { updatePetService } from "../../SharedServices/petServiceManagementService";

function UpdatePetService({petService, show, hide, setLoginState, postUpdatePetAction}){
    const [updating, setUpdating] = useState(false);
    const [errMsg, setErrMsg] = useState(undefined);
    const [successMsg, setSuccessMsg] = useState(false);
  
    const closeModal = function () {
        resetDefault();
        hide();
    };

    const resetDefault = function(){
        setErrMsg(undefined);
        setSuccessMsg(false);
        setUpdating(false);
    }

    const updateService = function(e){
        e.preventDefault();

        setErrMsg(undefined);
        setSuccessMsg(false);
        setUpdating(true);

        (async function(){
            try{
                const name = e.target.name.value;
                const serviceRate = parseFloat(e.target.serviceRate.value);
                const employeeRate = parseFloat(e.target.employeeRate.value);
                const description = e.target.description.value;
                const duration = parseInt(e.target.duration.value);
                const timeUnit = e.target.unit.value;

                var resp = await updatePetService(petService.id, name, serviceRate, employeeRate, description, duration, timeUnit);

                if (resp !== undefined && resp.status === 401){
                    setLoginState(false);
                    return;
                }

                const map = new Map();
                map.set('id', petService.id);
                map.set('name', name);
                map.set('rate', serviceRate);
                map.set('employeeRate', employeeRate);
                map.set('description', description);
                map.set('duration', duration);
                map.set('timeUnit', timeUnit);

                postUpdatePetAction(map)
                setSuccessMsg(true);
            }catch(e){
                setErrMsg(e.message);
            }finally{
                setUpdating(false);
            }
        })();
    }

    return(
        <>
            <Modal
                show={show}
                onHide={closeModal}
                dialogClassName="modal70"
                aria-labelledby="contained-modal-title-vcenter"                
                centered
            >
                <Modal.Header className="modal-header-color">
                    <Modal.Title id="contained-modal-title-vcenter">
                        Update Service
                    </Modal.Title>
                </Modal.Header>

                <Modal.Body>
                    <Form onSubmit={updateService}>
                        {errMsg !== undefined && <Alert variant="danger">{errMsg}</Alert>}
                        {successMsg && (
                            <Alert variant="success">Pet Service successfully updated.</Alert>
                        )}
                        <Row>
                            <Form.Group as={Col} md="6">
                                <Form.Label>Service Name</Form.Label>
                                <Form.Control
                                    placeholder="Service Name"
                                    name="name"
                                    defaultValue={petService === undefined ? undefined : petService.name}
                                    required
                                />                            
                            </Form.Group>
                            <Form.Group as={Col} md="3">
                                <Form.Label>Service Rate</Form.Label>
                                <Form.Control
                                    type="number"
                                    step="0.01"
                                    placeholder="Service Rate"
                                    name="serviceRate"
                                    min={0}
                                    defaultValue={petService === undefined ? undefined : petService.rate}
                                    required
                                />                              
                            </Form.Group>
                            <Form.Group as={Col} md="3">
                            <Form.Label>Employee Rate (%)</Form.Label>
                                <Form.Control
                                    type="number"
                                    placeholder="Employee Rate"
                                    name="employeeRate"
                                    step="0.01"
                                    min={0}
                                    max={100}
                                    defaultValue={petService === undefined ? undefined : petService.employeeRate}
                                    required
                                />
                            </Form.Group>
                        </Row><br />
                        <Row>
                            <Form.Group as={Col} md="3">
                                <Form.Label>Duration</Form.Label>
                                <Form.Control
                                    type="number"                            
                                    placeholder="Duration"
                                    name="duration"
                                    min={1}
                                    defaultValue={petService === undefined ? undefined : petService.duration}
                                    required
                                />                              
                            </Form.Group>
                            <Form.Group as={Col} md="3">
                                <Form.Label>Time Unit</Form.Label>
                                <Form.Select
                                    name="unit"
                                    required
                                    defaultValue={petService === undefined ? "Seconds" : petService.timeUnit}
                                >
                                    <option value="Seconds">Seconds</option>
                                    <option value="Minutes">Minutes</option>
                                    <option value="Hours">Hours</option>
                                </Form.Select>                        
                            </Form.Group>
                        </Row><br/>
                        <Row>
                            <Form.Group as={Col} md="12">
                                <Form.Label>Description</Form.Label>
                                <Form.Control
                                    placeholder="Description"
                                    name="description"
                                    as="textarea"
                                    rows={5}
                                    maxLength={2000}
                                    defaultValue={petService === undefined ? undefined : petService.description}
                                />                            
                            </Form.Group>
                        </Row><br/>
                        <UpdateEntityBtn updating={updating} closeModal={closeModal} />
                    </Form>
                </Modal.Body>
            </Modal>
        </>
    );
}

export default UpdatePetService;