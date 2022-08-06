import { useState } from "react";
import { Form, Button, Row, Col } from "react-bootstrap";

import AddPetModal from "./AddModal";

function PetManagement({ setLoginState }){
    const [showAddModal, setShowAddModal] = useState(false);  

    return(
        <>
            <h1>Pets Management</h1>
            <br/>
            <AddPetModal
                show={showAddModal}
                closeModal={() => setShowAddModal(false)}
            />
            <Row>
                <Form>
                <Row className="align-items-center">
                    <Col lg={4}>
                        <Form.Control
                            name="searchPet"
                            id="searchPet"
                            placeholder="Search pets by name and breed"
                        />
                    </Col>
                    <Col lg={6}>
                        <Button type="submit">Search</Button>
                    </Col>
                    <Col lg={2}>
                        <Button onClick={() => setShowAddModal(true)}>
                            Add Pet
                        </Button>
                    </Col>
                </Row>
                </Form>
            </Row>
        </>
    )
}

export default PetManagement;