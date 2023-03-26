import { useState, useEffect } from "react";
import { Modal, Form, Row, Col, Button, Spinner, Alert } from "react-bootstrap";
import Select from "react-select";

import { addEvent } from "../../SharedServices/jobEventService";
import { getPetServices, getPets, getEmployees } from "../../SharedServices/dropdownService";
import { ensureAddEventInformationProvided } from "../../SharedServices/inputValidationService";

function AddEventModal({show, handleHide, handleAddSuccess, setLoginState, eventDate, view}){
    const [errorMessage, setErrorMessage] = useState(undefined);
    const [successMessage, setSuccessMessage] = useState(undefined);
    const [loading, setLoading] = useState(false);
    const [disableBtns, setDisableBtns] = useState(false);
    const [validationMap, setValidationMap] = useState(new Map());

    const [employees, setEmployees] = useState([]);
    const [pets, setPets] = useState([]);
    const [petServices, setPetServices] = useState([]);
    
    useEffect(() => {
        (async function () {
            try {
                var resp = undefined;
                
                resp = await getEmployees();
                if (resp.status === 401) {
                    setLoginState(false);
                    return;
                }

                const employees = await resp.json();
                constructEmployeeOptions(employees);

                resp = await getPets();
                if (resp.status === 401) {
                    setLoginState(false);
                    return;
                }

                const pets = await resp.json();
                constructPetOptions(pets);

                resp = await getPetServices();
                if (resp.status === 401) {
                    setLoginState(false);
                    return;
                }

                const petServices = await resp.json();
                constructPetServiceOptions(petServices);

                setErrorMessage(undefined);
            } catch (e) {
                setErrorMessage(e.message);
            }
        })();
    }, []);

    const constructEmployeeOptions = (employees) => {
        const employeeOptions = [];
        for (var i = 0; i < employees.length; i++) {
            employeeOptions.push({ value: employees[i].id, label: employees[i].fullName });
        }

        setEmployees(employeeOptions);
    }

    const constructPetOptions = (pets) => {
        const petOptions = [];
        for (var i = 0; i < pets.length; i++) {
            petOptions.push({ value: pets[i].id, label: pets[i].name });
        }

        setPets(petOptions);
    };

    const constructPetServiceOptions = (services) => {
        const serviceOptions = [];
        for (var i = 0; i < services.length; i++) {
            serviceOptions.push({ value: services[i].id, label: services[i].name });
        }

        setPetServices(serviceOptions);
    }

    const closeModal = function () {
        resetStates();
        handleHide();
    };

    const resetStates = function () {
        setValidationMap(new Map());
        setErrorMessage(undefined);
        setSuccessMessage(undefined);
        setLoading(false);
        setDisableBtns(false);
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

        var inputValidations = ensureAddEventInformationProvided (employeeId, petId, petServiceId, e.target.hour.value, e.target.minute.value, e.target.ampm.value);
        if (inputValidations.size > 0) {
            setValidationMap(inputValidations);
            return;
        }

        setValidationMap(new Map());

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
                            <Select
                                name="employee"
                                defaultValue={{ label: "Select Employee", value: 0 }}
                                options={employees}
                                isInvalid={validationMap.has("employeeId")}
                            />
                            <div className="dropdown-invalid">
                                {" "}
                                {validationMap.get("employeeId")}
                            </div>
                        </Col>
                        </Form.Group>
                        <br />
                        <Form.Group as={Row}>
                        <Form.Label column lg={3}>
                            Pet:
                        </Form.Label>
                        <Col lg={9}>
                            <Select
                                name="pet"
                                defaultValue={{ label: "Select Pet", value: 0 }}
                                options={pets}
                                isInvalid={validationMap.has("petId")}
                            />
                            <div className="dropdown-invalid">
                                {" "}
                                {validationMap.get("petId")}
                            </div>
                        </Col>
                        </Form.Group>
                        <br/>
                        <Form.Group as={Row}>
                        <Form.Label column lg={3}>
                            Pet Service:
                        </Form.Label>
                        <Col lg={9}>
                            <Select
                                name="petService"
                                defaultValue={{ label: "Select Pet Service", value: 0 }}
                                options={petServices}
                                isInvalid={validationMap.has("petServiceId")}
                            />
                            <div className="dropdown-invalid">
                                {" "}
                                {validationMap.get("petServiceId")}
                            </div>
                        </Col>
                        </Form.Group>
                        <br />
                        {view && (<Form.Group as={Row}>
                        <Form.Label column lg={3}>
                            Time:
                        </Form.Label>
                        <Col lg={3}>
                            <Form.Select type="select" name="hour" isInvalid={validationMap.has("hour")}>
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
                            <Form.Control.Feedback type="invalid">
                                {validationMap.get("hour")}
                            </Form.Control.Feedback>
                        </Col>
                        <Col lg={3}>
                            <Form.Select type="select" name="minute" isInvalid={validationMap.has("minute")}>
                                <option value="00">00</option>
                                <option value="15">15</option>
                                <option value="30">30</option>
                                <option value="45">45</option>
                            </Form.Select>
                            <Form.Control.Feedback type="invalid">
                                {validationMap.get("minute")}
                            </Form.Control.Feedback>
                        </Col>
                        <Col lg={3}>
                            <Form.Select type="select" name="ampm" isInvalid={validationMap.has("ampm")}>
                                <option value="am">AM</option>
                                <option value="pm">PM</option>
                            </Form.Select>
                            <Form.Control.Feedback type="invalid">
                                {validationMap.get("ampm")}
                            </Form.Control.Feedback>
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