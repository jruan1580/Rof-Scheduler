import { Modal, Button, Row, Form, Col, Alert, Spinner } from "react-bootstrap";

import { useState } from "react";

import { addHoliday } from "../../../SharedServices/holidayAndHolidayRateService";

import "../holiday.css";

function AddHoliday({ show, handleHide, setLoginState }){
    const [loading, setLoading] = useState(false);
    const [errMsg, setErrMsg] = useState(undefined);
    const [successMsg, setSuccessMsg] = useState(false);
    const [disableBtns, setDisableBtns] = useState(false);

    const closeModal = function () {
        setErrMsg(undefined);
        handleHide();
    };

    const handleAddHoliday = function(e){
        e.preventDefault();

        successMsg(false);
        setErrMsg(undefined);
        setLoading(true);      

        (async function(){
            try{
                const name = e.target.name.value;
                const month = parseInt(e.target.month.value);
                const day = parseInt(e.target.day.value);

                var resp = await addHoliday(name, month, day);
                if (resp !== undefined && resp.status === 401){
                    setLoginState(false);
                    return;
                }
    
                setErrMsg(undefined);
                setDisableBtns(true);
    
                setSuccessMsg(true);
            }catch(e){
                setErrMsg(e.message);
            }finally{
                setLoading(false);
            }
        })();
    }

    return (
        <>
            <Modal
                show={show}
                onHide={closeModal}
                aria-labelledby="contained-modal-title-vcenter"                
                centered
            >
                <Modal.Header className="modal-header-color">
                    <Modal.Title id="contained-modal-title-vcenter">
                        Add Holiday
                    </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form onSubmit={handleAddHoliday}>
                        {errMsg !== undefined && <Alert variant="danger">{errMsg}</Alert>}
                        {successMsg && (
                            <Alert variant="success">
                                Holiday successfully added! Page will reload in 3 seconds and new Holiday will be available....
                            </Alert>
                        )}
                        <Row>
                            <Form.Group as={Col} md="6">
                                <Form.Label>Holiday Name</Form.Label>
                                <Form.Control
                                    placeholder="Holiday Name"
                                    name="name"
                                    required
                                />                            
                            </Form.Group>
                            <Form.Group as={Col} md="3">
                                <Form.Label>Month</Form.Label>
                                <Form.Control
                                    type="number"
                                    placeholder="Month"
                                    name="month"
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
                                    min={1}
                                    max={31}
                                    required
                                />
                            </Form.Group>
                        </Row> <br />
                        <hr></hr>
                        {(loading || disableBtns) && (
                            <Button
                                type="button"
                                variant="danger"
                                className="float-end ms-2"
                                disabled
                            >
                                Cancel
                            </Button>
                            )}
                            {!loading && !disableBtns && (
                            <Button
                                type="button"
                                variant="danger"
                                onClick={() => closeModal()}
                                className="float-end ms-2"
                            >
                                Cancel
                            </Button>
                            )}
                            {(loading || disableBtns) && (
                            <Button variant="primary" className="float-end" disabled>
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
                            {!loading && !disableBtns && (
                            <Button type="submit" className="float-end">
                                Add
                            </Button>
                        )}
                    </Form>
                </Modal.Body>
            </Modal>
        </>
    );
}

export default AddHoliday;