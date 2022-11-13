import { Modal, Row, Form, Col, Button, Spinner, Alert } from "react-bootstrap";

import { useState } from "react";

import "../../../SharedCSS/modal.css";
import { updateHoliday } from "../../../SharedServices/holidayAndHolidayRateService";

function UpdateHoliday({holiday, show, hide, setLoginState, postUpdatePetAction}){
    const [updating, setUpdating] = useState(false);
    const [errMsg, setErrMsg] = useState(undefined);
    const [successMsg, setSuccessMsg] = useState(false);
  
    const closeModal = function () {
      setErrMsg(undefined);
      setSuccessMsg(false);
      hide();
    };

    const handleUpdateHoliday  = function(e){
        e.preventDefault();

        setErrMsg(undefined);
        setSuccessMsg(false);
        setUpdating(true);

        (async function(){
            try{
                const name = e.target.name.value;
                const month = parseInt(e.target.month.value);
                const day = parseInt(e.target.day.value);
                
                var resp = await updateHoliday(holiday.id, name, month, day);

                if (resp !== undefined && resp.status === 401){
                    setLoginState(false);
                    return;
                }

                const map = new Map();
                map.set('id', holiday.id);
                map.set('name', name);
                map.set('month', month);
                map.set('day', day);

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
                aria-labelledby="contained-modal-title-vcenter"                
                centered
            >
                <Modal.Header className="modal-header-color">
                    <Modal.Title id="contained-modal-title-vcenter">
                        Update Holiday
                    </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form onSubmit={handleUpdateHoliday}>
                        {errMsg !== undefined && <Alert variant="danger">{errMsg}</Alert>}
                        {successMsg && (
                            <Alert variant="success">Holiday successfully updated.</Alert>
                        )}

                        <Row>
                            <Form.Group as={Col} md="6">
                                <Form.Label>Holiday Name</Form.Label>
                                <Form.Control
                                    placeholder="Holiday Name"
                                    name="name"
                                    defaultValue={holiday === undefined ? "" : holiday.name}
                                    required
                                />                            
                            </Form.Group>
                            <Form.Group as={Col} md="3">
                                <Form.Label>Month</Form.Label>
                                <Form.Control
                                    type="number"
                                    placeholder="Month"
                                    name="month"
                                    defaultValue={holiday === undefined ? "" : holiday.month}
                                    min={1}
                                    max={12}
                                    required
                                />                              
                            </Form.Group>
                            <Form.Group as={Col} md="3">
                            <Form.Label>Day</Form.Label>
                                <Form.Control
                                    type="number"
                                    placeholder="Day"
                                    name="day"
                                    defaultValue={holiday === undefined ? "" : holiday.day}
                                    min={1}
                                    max={31}
                                    required
                                />
                            </Form.Group>
                        </Row><br/>

                        <hr></hr>
                        {updating && (
                            <Button
                                type="button"
                                variant="danger"
                                className="float-end ms-2"
                                disabled
                            >
                                Cancel
                            </Button>
                        )}
                        {!updating && (
                            <Button
                                type="button"
                                variant="danger"
                                onClick={() => closeModal()}
                                className="float-end ms-2"
                            >
                                Cancel
                            </Button>
                        )}
                        {updating && (
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
                        {!updating && (
                            <Button type="submit" className="float-end">
                                Update
                            </Button>
                        )}
                    </Form>
                </Modal.Body>
            </Modal>
        </>
    );
}

export default UpdateHoliday;