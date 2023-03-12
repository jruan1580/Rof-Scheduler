import { Modal, Row, Form, Col, Button, Spinner, Alert } from "react-bootstrap";
import { useEffect, useState } from "react";
import Select from "react-select";

import { getPetServices, getPets, getEmployees } from "../../SharedServices/dropdownService";
import { updateEvent } from "../../SharedServices/jobEventService";

function UpdateEventModal({event, show, handleHide, setLoginState, hour, minute, ampm, date}){
    const [errorMessage, setErrorMessage] = useState(undefined);
    const [successMessage, setSuccessMessage] = useState(undefined);

    const [employees, setEmployees] = useState([]);
    const [pets, setPets] = useState([]);
    const [petServices, setPetServices] = useState([]);
    
    useEffect(() => {
        (async function () {
            try {
                if (event === undefined){
                    return;
                }

                var resp = undefined;
                
                // resp = await getEmployees();
                // if (resp.status === 401) {
                //     setLoginState(false);
                //     return;
                // }

                // const employees = await resp.json();
                // constructEmployeeOptions(employees);

                // resp = await getPets();
                // if (resp.status === 401) {
                //     setLoginState(false);
                //     return;
                // }

                // const pets = await resp.json();
                // constructPetOptions(pets);

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
    }, [event]);
    
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
        setErrorMessage(undefined);
        setSuccessMessage(undefined);
        handleHide();
    };

    const updateEventSubmit = (e) => {
        console.log("Update");
        e.preventDefault();
        setErrorMessage(undefined);
        setSuccessMessage(false);

        var employeeId = e.target.employee.value;
        var petId = e.target.pet.value;
        var petServiceId = e.target.petService.value;
        var eventStart = undefined;
        var isComplete = e.target.isComplete.value;
        var isCanceled = false;

        console.log(isComplete);

        var hour = undefined;
        var eventTime = undefined;
        var eventDate = e.target.date.value;

        if(e.target.ampm.value.toLowerCase() === "am" && e.target.hour.value === "12"){
            hour = "00";
            eventTime = hour + ":" + e.target.minute.value;
        } else if(e.target.ampm.value.toLowerCase() === "pm"){
            hour = parseInt(e.target.hour.value) + 12;
            eventTime = hour + ":" + e.target.minute.value;
        }else{
            eventTime = e.target.hour.value + ":" + e.target.minute.value;
        }

        eventStart = eventDate + "T" + eventTime + ":00";
        
        // (async function () {
        //     try {
        //         const resp = await updateEvent(employeeId, petId, petServiceId, eventStart, isComplete, isCanceled);

        //         if (resp.status === 401) {
        //             setLoginState(false);
        //             return;
        //         }

        //         setErrorMessage(undefined);
        //         setSuccessMessage(true);
        //     } catch (e) {
        //         setErrorMessage(e.message);
        //         return;
        //     }
        // })();
    }

    const deleteEvent = (id) => {
        console.log("Delete");
    }

    const hourOptions = [
        {value: "01", label: "01"},
        {value: "02", label: "02"},
        {value: "03", label: "03"},
        {value: "04", label: "04"},
        {value: "05", label: "05"},
        {value: "06", label: "06"},
        {value: "07", label: "07"},
        {value: "08", label: "08"},
        {value: "09", label: "09"},
        {value: "10", label: "10"},
        {value: "11", label: "11"},
        {value: "12", label: "12"}
    ]

    const minOptions = [
        {value: "00", label: "00"},
        {value: "15", label: "15"},
        {value: "30", label: "30"},
        {value: "45", label: "45"}
    ]

    const ampmOptions = [
        {value: "am", label: "AM"},
        {value: "pm", label: "PM"}
    ]

    const completeOptions = [
        {value: true, label: "Yes"},
        {value: false, label: "No"}
    ]

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
                                {event !== undefined && (
                                    <Select
                                        name="employee"
                                        options={employees}
                                        defaultValue={{
                                            label: event.extendedProps.employee,
                                            value: event.extendedProps.employeeId,
                                        }}
                                    />
                                )}
                            </Col>
                        </Form.Group>
                        <br />
                        <Form.Group as={Row}>
                            <Form.Label column lg={3}>Pet:</Form.Label>
                            <Col lg={9}>
                                {event !== undefined && (
                                    <Select
                                        name="pet"
                                        options={pets}
                                        defaultValue={{
                                            label: event.extendedProps.pet,
                                            value: event.extendedProps.petId,
                                        }}
                                    />
                                )}
                            </Col>
                        </Form.Group>
                        <br />
                        <Form.Group as={Row}>
                            <Form.Label column lg={3}>Pet Service:</Form.Label>
                            <Col lg={9}>
                                {event !== undefined && (
                                    <Select
                                        name="petService"
                                        options={petServices}
                                        defaultValue={{
                                            label: event.title,
                                            value: event.extendedProps.petServiceId,
                                        }}
                                    />
                                )}
                            </Col>
                        </Form.Group>                        
                        <br />
                        <Form.Group as={Row}>
                            <Form.Label column lg={3}>Date:</Form.Label>
                            <Col lg={9}>
                                <Form.Control
                                    type="date"
                                    defaultValue={event === undefined ? "" : date}
                                    name="date"
                                />
                            </Col>
                        </Form.Group>
                        <br />
                        <Form.Group as={Row}>
                            <Form.Label column lg={3}>Time:</Form.Label>
                            <Col lg={3}>
                                {event !== undefined && (
                                    <Select
                                        name="hour"
                                        options={hourOptions}
                                        defaultValue={{
                                            label: hour,
                                            value: hour,
                                        }}
                                    />
                                )}
                            </Col>
                            <Col lg={3}>
                                {event !== undefined && (
                                    <Select
                                        name="minute"
                                        options={minOptions}
                                        defaultValue={{
                                            label: minute,
                                            value: minute,
                                        }}
                                    />
                                )}
                            </Col>
                            <Col lg={3}>
                                {event !== undefined && (
                                    <Select
                                        name="ampm"
                                        options={ampmOptions}
                                        defaultValue={{
                                            label: ampm,
                                            value: ampm,
                                        }}
                                    />
                                )}
                            </Col>
                        </Form.Group>
                        <br />
                        <Form.Group as={Row}>
                            <Form.Label column lg={3}>Completed:</Form.Label>
                            <Col lg={3}>
                                {event !== undefined && (
                                    <Select
                                        name="isComplete"
                                        options={completeOptions}
                                        defaultValue={{
                                            label: event.extendedProps.isComplete === true ? "Yes" : "No",
                                            value: event.extendedProps.isComplete,
                                        }}
                                    />
                                )}
                            </Col>
                        </Form.Group>
                        <hr />
                        <Button type="button" variant="secondary" onClick={() => deleteEvent()} className="float-start me-2">Delete</Button>
                        <Button type="button" variant="danger" onClick={() => closeModal()} className="float-end ms-2">Cancel</Button>
                        <Button type="submit" variant="primary" className="float-end ms-2">Update</Button>
                    </Form>
                </Modal.Body>
          </Modal>
        </>
    )
}

export default UpdateEventModal;