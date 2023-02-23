import { Modal, Row, Form, Col, Button, Spinner, Alert } from "react-bootstrap";
import { useEffect, useState } from "react";

function UpdateEventModal(event, show, handleHide, setLoginState){
    const [errorMessage, setErrorMessage] = useState(undefined);
    const [successMessage, setSuccessMessage] = useState(undefined);

    const closeModal = function () {
        setErrorMessage(undefined);
        setSuccessMessage(undefined);
        // setLoading(false);
        // setDisableBtns(false);
        handleHide();
    };

    const updateEventSubmit = () => {

    }

    return(
        <>
            <Modal show={show} onHide={closeModal}>
                <Modal.Header className="modal-header-color" closeButton>
                    <Modal.Title>Update Event</Modal.Title>
                </Modal.Header>

                <Modal.Body>
                    <Form onSubmit={updateEventSubmit}>
                        {errorMessage !== undefined && (<Alert variant="danger">{errorMessage}</Alert>)}
                        {successMessage && ( <Alert variant="success">Event updated successfully.</Alert>)}

                        <Form.Group as={Row}>
                            <Form.Label column lg={3}>Employee:</Form.Label>
                            <Col lg={9}>
                                <Form.Select type="select" name="employee">
                                    <option value=""></option>
                                </Form.Select>
                            </Col>
                        </Form.Group>
                        <br />
                        <Form.Group as={Row}>
                            <Form.Label column lg={3}>Pet:</Form.Label>
                            <Col lg={9}>
                                <Form.Select type="select" name="pet">
                                    <option value=""></option>
                                </Form.Select>
                            </Col>
                        </Form.Group>
                        <br />
                        <Form.Group as={Row}>
                            <Form.Label column lg={3}>Pet Service:</Form.Label>
                            <Col lg={9}>
                                <Form.Select type="select" name="petService">
                                    <option value=""></option>
                                </Form.Select>
                            </Col>
                        </Form.Group>
                        <br />
                        <Form.Group as={Row}>
                            <Form.Label column lg={3}>Time:</Form.Label>
                            <Col lg={3}>
                            <Form.Select type="select" name="hour">
                            <option value="01">01</option>
                            <option value="02">02</option>
                            <option value="03">03</option>
                            <option value="04">04</option>
                            <option value="05">05</option>
                            <option value="06">06</option>
                            <option value="07">07</option>
                            <option value="08">08</option>
                            <option value="09">09</option>
                            <option value="10">10</option>
                            <option value="11">11</option>
                            <option value="12">12</option>
                            </Form.Select>
                            </Col>
                            <Col lg={3}>
                                <Form.Select type="select" name="minute">
                                <option value="00">00</option>
                                <option value="15">15</option>
                                <option value="30">30</option>
                                <option value="45">45</option>
                                </Form.Select>
                            </Col>
                            <Col lg={3}>
                                <Form.Select type="select" name="ampm">
                                <option value="am">AM</option>
                                <option value="pm">PM</option>
                                </Form.Select>
                            </Col>
                        </Form.Group>
                        <hr />
                        <Button type="button" variant="danger" className="float-start ms-2">Delete</Button>
                        <Button type="button" variant="secondary" className="float-start ms-2">Complete</Button>
                        <Button type="button" variant="danger" className="float-end ms-2">Cancel</Button>
                        <Button type="submit" variant="primary" className="float-end ms-2">Update</Button>
                    </Form>
                </Modal.Body>
          </Modal>
        </>
    )
}

export default UpdateEventModal;