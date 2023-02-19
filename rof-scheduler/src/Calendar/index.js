import FullCalendar from '@fullcalendar/react' // must go before plugins
import dayGridPlugin from '@fullcalendar/daygrid' // a plugin!
import interactionPlugin from "@fullcalendar/interaction" // needed for dayClick
import timeGridPlugin from '@fullcalendar/timegrid'
import { useState, useRef } from "react";
import { Modal, Form, Row, Col, Button, Alert } from "react-bootstrap";

import { getPetServices, getPets } from "../SharedServices/dropdownService";

function Calendar({ setLoginState }) {
  const calendarRef = useRef();
  const [showAddModal, setShowAddModal] = useState(false);
  const [employees, setEmployees] = useState([]);
  const [pets, setPets] = useState([]);
  const [petServices, setPetServices] = useState([]);
  const [errorMessage, setErrorMessage] = useState(undefined);

  const handleEventClick = (arg) => {
      console.log(arg);
      alert(arg);
  }

  const handleDateSelect = (selectInfo) => {
    console.log(selectInfo);
    constructPetOptions();
    constructPetServiceOptions();
    setShowAddModal(true);
  }

  //get pet services for dropdown
  const constructPetServiceOptions = () => {
    (async function () {
      try {
        const resp = await getPetServices();
        if (resp.status === 401) {
          setLoginState(false);
          return;
        }

        const petServices = await resp.json();
        setPetServices(petServices);
        
      } catch (e) {
        setErrorMessage(e.message);
      }
    })();
  }

  //get pets for dropdown
  const constructPetOptions = () => {
    (async function () {
      try {
        const resp = await getPets();
        if (resp.status === 401) {
          setLoginState(false);
          return;
        }

        const pets = await resp.json();
        setPets(pets);
        
      } catch (e) {
        setErrorMessage(e.message);
      }
    })();
  };

  //reset everything when we close modal
  const closeModal = function () {
    setErrorMessage(undefined);
    setShowAddModal(false);
  };

  const addEventSubmit = (e) => {
    e.preventDefault();

    setErrorMessage(undefined);

    (async function () {
      try {
        
      } catch (e) {
        setErrorMessage(e.message);
        return;
      }
    })();
  };

  return(
      <>
          <FullCalendar
              plugins={[dayGridPlugin, timeGridPlugin, interactionPlugin]}
              headerToolbar={{
                left: 'prev,next today',
                center: 'title',
                right: 'dayGridMonth,timeGridWeek,timeGridDay'
              }}
              initialView="dayGridMonth"
              editable={true}
              selectable={true}    
              selectMirror={true}
              select={handleDateSelect}
              eventClick={handleEventClick}            
              events={[
                  { title: 'event 1', start: '2022-04-29T05:00:00', end: '2022-04-29T07:00:00'},
                  { title: 'event 2', start: '2022-04-29T05:00:00', end: '2022-04-29T06:00:00' }
                ]}
          />
          
          <Modal show={showAddModal} onHide={closeModal}>
            <Modal.Header className="modal-header-color" closeButton>
                <Modal.Title>Add Event</Modal.Title>
            </Modal.Header>

            <Modal.Body>
              <Form onSubmit={addEventSubmit}>
                {errorMessage !== undefined && <Alert variant="danger">{errorMessage}</Alert>}

                <Form.Group as={Row}>
                  <Form.Label column lg={3}>
                    Employee:
                  </Form.Label>
                  <Col lg={9}>
                    <Form.Select type="select" name="employees">
                      {/* {employees.map((employee) => {
                        return (
                          <option key={employee.id} value={employee.id}>
                            {employee.petTypeName}
                          </option>
                        );
                      })} */}
                    </Form.Select>
                  </Col>
                </Form.Group>
                <br />
                <Form.Group as={Row}>
                  <Form.Label column lg={3}>
                    Pet:
                  </Form.Label>
                  <Col lg={9}>
                    <Form.Select type="select" name="pets">
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
                <hr></hr>
                <Button type="submit" className="float-end">
                  Add
                </Button>
              </Form>
            </Modal.Body>
          </Modal>
      </>
  )
}

export default Calendar;