import { useState } from "react";
import { Modal, Form, Row, Col, Button, Spinner, Alert } from "react-bootstrap";
import { addEvent } from "../../SharedServices/jobEventService";

function AddEventModal({show, handleHide, handleAddSuccess, setLoginState, eventDate, view, employees, pets, petServices}){
    const [errorMessage, setErrorMessage] = useState(undefined);
    const [successMessage, setSuccessMessage] = useState(undefined);
    const [loading, setLoading] = useState(false);
    const [disableBtns, setDisableBtns] = useState(false);

    const closeModal = function () {
        setErrorMessage(undefined);
        setSuccessMessage(undefined);
        setLoading(false);
        setDisableBtns(false);
        handleHide();
    };

    const addEventSubmit = (e) => {
        e.preventDefault();

        setErrorMessage(undefined);

        const employeeId = parseInt(e.target.employee.value);
        const petId = parseInt(e.target.pet.value);
        const petServiceId = parseInt(e.target.petService.value);
        var eventStart = undefined;

        var eventTime = undefined; 
        
        if(view){
            var hour = undefined;

            if(e.target.ampm.value === "am" && e.target.hour.value === "12"){
                hour = "00";
                eventTime = hour + ":" + e.target.minute.value;
            } else if(e.target.ampm.value === "pm"){
                hour = parseInt(e.target.hour.value) + 12;
                eventTime = hour + ":" + e.target.minute.value;
            }else{
                eventTime = e.target.hour.value + ":" + e.target.minute.value;
            }
        }

        if(eventTime !== undefined){
            eventStart = eventDate + "T" + eventTime + ":00";
        }else{
            eventStart = eventDate;
        }

        setLoading(true);

        (async function () {
            try {
                const resp = await addEvent(employeeId, petId, petServiceId, eventStart);

                if (resp.status === 401) {
                setLoginState(false);
                return;
                }

                setErrorMessage(undefined);
                setSuccessMessage(true);
                setDisableBtns(true);
                handleAddSuccess();
            } catch (e) {
                setErrorMessage(e.message);
                return;
            }finally{
                setLoading(false);
            }
        })();
    };

    return(
        <>
            <Modal show={show} onHide={closeModal}>
                <Modal.Header className="modal-header-color" closeButton>
                    <Modal.Title>Add Event</Modal.Title>
                </Modal.Header>

                <Modal.Body>
                    <Form onSubmit={addEventSubmit}>
                        {errorMessage !== undefined && <Alert variant="danger">{errorMessage}</Alert>}
                        {successMessage && (
                            <Alert variant="success">
                                Event successfully added! Page will reload in 3 seconds....
                            </Alert>
                        )}
                        <Form.Group as={Row}>
                        <Form.Label column lg={3}>
                            Employee:
                        </Form.Label>
                        <Col lg={9}>
                            <Form.Select type="select" name="employee">
                            {employees.map((employee) => {
                                return (
                                <option key={employee.id} value={employee.id}>
                                    {employee.fullName}
                                </option>
                                );
                            })}
                            </Form.Select>
                        </Col>
                        </Form.Group>
                        <br />
                        <Form.Group as={Row}>
                        <Form.Label column lg={3}>
                            Pet:
                        </Form.Label>
                        <Col lg={9}>
                            <Form.Select type="select" name="pet">
                            {pets.map((pet) => {
                                return (
                                <option key={pet.id} value={pet.id}>
                                    {pet.name}
                                </option>
                                );
                            })}
                            </Form.Select>
                        </Col>
                        </Form.Group>
                        <br/>
                        <Form.Group as={Row}>
                        <Form.Label column lg={3}>
                            Pet Service:
                        </Form.Label>
                        <Col lg={9}>
                            <Form.Select type="select" name="petService">
                            {petServices.map((petService) => {
                                return (
                                <option key={petService.id} value={petService.id}>
                                    {petService.name}
                                </option>
                                );
                            })}
                            </Form.Select>
                        </Col>
                        </Form.Group>
                        <br />
                        {view && (<Form.Group as={Row}>
                        <Form.Label column lg={3}>
                            Time:
                        </Form.Label>
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
                        </Form.Group>)}
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
                            <Button variant="primary" className="float-end ms-2" disabled>
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
                            <Button type="submit" className="float-end ms-2">
                                Add
                            </Button>
                        )}
                    </Form>
                </Modal.Body>
          </Modal>
        </>
    )
}

export default AddEventModal;