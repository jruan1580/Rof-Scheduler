import { Modal, Button, Row, Form, Col, Alert, Spinner } from "react-bootstrap";

import "../../SharedCSS/modal.css";

import { useState } from "react";
import AddEntityBtn from "../../SharedComponents/AddEntityBtn";

import { addNewPetService } from "../../SharedServices/petServiceManagementService";

function AddPetService({ show, handleHide, setLoginState, reloadAfterThreeSeconds }){
    const [errMsg, setErrMsg] = useState(undefined);
    const [successMsg, setSuccessMsg] = useState(false);
    const [loading, setLoading] = useState(false);
    const [disableBtns, setDisableBtns] = useState(false);

    const closeModal = function () {
        resetDefaults();
        handleHide();
    };

    const addService = function(e){
        e.preventDefault();

        setSuccessMsg(false);
        setErrMsg(undefined);
        setLoading(true);     
        setDisableBtns(false);
        
        (async function(){
            try{
                const name = e.target.name.value;
                const serviceRate = parseFloat(e.target.serviceRate.value);
                const employeeRate = parseFloat(e.target.employeeRate.value);
                const description = e.target.description.value;
                const duration = parseInt(e.target.duration.value);
                const timeUnit = e.target.unit.value;

                var resp = await addNewPetService(name, serviceRate, employeeRate, description, duration, timeUnit);
                if (resp !== undefined && resp.status === 401){
                    setLoginState(false);
                    return;
                }
    
                setErrMsg(undefined);
                setDisableBtns(true);
    
                setSuccessMsg(true);
                reloadAfterThreeSeconds();
            }catch(e){
                setErrMsg(e.message);
            }finally{
                setLoading(false);
            }
        })();
    }

    const resetDefaults = function(){
        setSuccessMsg(false);
        setErrMsg(undefined);
        setLoading(false);
        setDisableBtns(false);
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
                        Add Service
                    </Modal.Title>
                </Modal.Header>
                
                <Modal.Body>
                    <Form onSubmit={addService}>
                        {errMsg !== undefined && <Alert variant="danger">{errMsg}</Alert>}
                        {successMsg && (
                            <Alert variant="success">
                                Service successfully added! Page will reload in 3 seconds and new service will be available....
                            </Alert>
                        )}

                        <Row>
                            <Form.Group as={Col} md="6">
                                <Form.Label>Service Name</Form.Label>
                                <Form.Control
                                    placeholder="Service Name"
                                    name="name"
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
                                required
                            />                              
                        </Form.Group>
                        <Form.Group as={Col} md="3">
                            <Form.Label>Time Unit</Form.Label>
                            <Form.Select
                                name="unit"
                                required
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
                                />                            
                            </Form.Group>
                        </Row><br/>
                       <AddEntityBtn loading={loading} disableBtns={disableBtns} closeModal={closeModal}/>
                    </Form>
                </Modal.Body>
            </Modal>
        </>
    );
}

export default AddPetService;