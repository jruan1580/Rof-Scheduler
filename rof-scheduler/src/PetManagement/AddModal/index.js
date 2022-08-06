import { useState } from "react";
import { Modal, Form, Row, Col, Button } from "react-bootstrap";
import Select from 'react-select';

function AddPetModal({show, closeModal}){
    const [petTypeSelected, setPetTypeSelected] = useState(1);

    const breed = [
        { label: 'Golden Retriever', value: 1 },
        { label: 'Husky', value: 2 }
      ];
      
    const addPet = (e) =>{
        e.preventDefault();

        console.log(e.target.breed.value);
        console.log(e.target.dob.value);
        console.log(e);
    }
    return(
        <>
            {
                //have not selected pet type
                petTypeSelected === undefined &&
                <Modal
                    show={show}
                    onHide={closeModal}
                >
                    <Modal.Header className="modal-header-color" closeButton>
                        <Modal.Title>
                            Add Pet
                        </Modal.Title>
                    </Modal.Header>

                    <Modal.Body>
                        <Form>
                            <Form.Group as={Row}>
                                <Form.Label column lg={3} >Pet Type:</Form.Label>
                                <Col lg={9}>
                                    <Form.Select
                                        type="select"
                                        placeholder="petType"
                                        name="petType"
                                    >
                                        <option value="dog">Dog</option>
                                        <option value="cat">Cat</option>
                                    </Form.Select>
                                </Col>                                             
                            </Form.Group>
                            <br />
                            <hr></hr>
                            <Button type="button" className="float-end">
                                Next
                            </Button>
                        </Form>
                        
                    </Modal.Body>
                    
                </Modal>
            }
            {
                petTypeSelected !== undefined &&
                <Modal
                    show={show}
                    onHide={closeModal}
                    dialogClassName="add-modal80"
                >
                    <Modal.Header className="modal-header-color" closeButton>
                        <Modal.Title>
                            Add Pet
                        </Modal.Title>
                    </Modal.Header>
                      <Modal.Body>
                        <Form onSubmit={addPet}>
                            <h4>Pet Information</h4>
                            <br />

                            <Row>
                                <Form.Group as={Col} lg={3}>
                                    <Form.Label>Name</Form.Label>
                                    <Form.Control 
                                        placeholder="Pet Name"
                                        name="petName"
                                    />
                                </Form.Group>
                                
                                <Form.Group as={Col} lg={3}>
                                    <Form.Label>Breed</Form.Label>
                                    <Select
                                        name="breed"
                                        options={breed}
                                    />                             
                                </Form.Group>

                                <Form.Group as={Col} lg={3}>
                                    <Form.Label>DOB</Form.Label>
                                    <Form.Control 
                                        type="date"
                                        placeholder="DOB"
                                        name="dob"
                                    />
                                </Form.Group>

                                <Form.Group as={Col} lg={3}>
                                    <Form.Label>Weight</Form.Label>
                                    <Form.Control 
                                        type="number"
                                        placeholder="weight"
                                        name="weight"
                                    />
                                </Form.Group>
                            </Row><br/>
                           
                            <Row>
                                <Form.Group as={Col} lg={12}>
                                    <Form.Label>Additional Information (Optional)</Form.Label>
                                    <Form.Control 
                                        placeholder="Additional Information"
                                        name="additionalInfo"
                                        as="textarea" 
                                        rows={5}
                                    />
                                </Form.Group>
                            </Row><br/>
                            
                            <h4>Vaccines</h4>
                            <br />
                            <Row>
                                <Form.Group as={Col} lg={3}>
                                    <Form.Check
                                        label="Bordetella"
                                        name="group1"
                                        type="checkbox"
                                    />
                                    <Form.Check
                                        label="2"
                                        name="group1"
                                        type="checkbox"
                                    />
                                </Form.Group>
                                <Form.Group as={Col} lg={3}>
                                    <Form.Check
                                        label="Bordetella"
                                        name="group1"
                                        type="checkbox"
                                    />
                                    <Form.Check
                                        label="2"
                                        name="group1"
                                        type="checkbox"
                                    />
                                </Form.Group>
                                <Form.Group as={Col} lg={3}>
                                    <Form.Check
                                        label="Bordetella"
                                        name="group1"
                                        type="checkbox"
                                    />
                                    <Form.Check
                                        label="2"
                                        name="group1"
                                        type="checkbox"
                                    />
                                </Form.Group>
                                <Form.Group as={Col} lg={3}>
                                    <Form.Check
                                        label="Bordetella"
                                        name="group1"
                                        type="checkbox"
                                    />
                                    <Form.Check
                                        label="2"
                                        name="group1"
                                        type="checkbox"
                                    />
                                </Form.Group>
                                
                            </Row>

                            <br />
                            <hr></hr>
                            <Button type="submit" className="float-end">
                                Create
                            </Button>
                        </Form>
                        
                    </Modal.Body>
                </Modal>
            }
            
        </>
    )
}

export default AddPetModal;