import { useState } from "react";
import { Modal, Form, Row, Col, Button } from "react-bootstrap";

function AddPetModal({show, closeModal}){
    const [petTypeSelected, setPetTypeSelected] = useState(undefined);

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
                    aria-labelledby="contained-modal-title-vcenter"
                    dialogClassName="add-modal80"
                    centered
                >
                    <Modal.Header className="modal-header-color" closeButton>
                        <Modal.Title>
                            Add Pet
                        </Modal.Title>
                    </Modal.Header>
                    
                </Modal>
            }
            
        </>
    )
}

export default AddPetModal;