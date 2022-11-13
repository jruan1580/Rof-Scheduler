import { Modal, Row, Form, Col, Alert } from "react-bootstrap";

import { useState } from "react";

import { addHoliday } from "../../../SharedServices/holidayAndHolidayRateService";

import "../../../SharedCSS/modal.css";
import AddEntityBtn from "../../../SharedComponents/AddEntityBtn";

function AddHoliday({ show, handleHide, setLoginState, reloadAfterThreeSeconds }){
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

        setSuccessMsg(false);
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
                reloadAfterThreeSeconds();
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

                        <AddEntityBtn loading={loading} disableBtns={disableBtns} closeModal={closeModal}/>                      
                    </Form>
                </Modal.Body>
            </Modal>
        </>
    );
}

export default AddHoliday;