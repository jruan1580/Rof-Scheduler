import { Form, Button, Row, Col, Alert } from 'react-bootstrap';
import GenericUserTable from '../SharedComponents/UserTable';

function Employee(){
    return(
            <>
                <h1>Employee Management</h1><br/>
                <Row>
                    <Form>
                        <Row className="align-items-center">
                            <Col lg={4}>            
                                <Form.Control id="searchEmployee" placeholder="Search employee by name, email..etc." />
                            </Col>
                            <Col lg={6}>
                                <Button type="submit">Search</Button>
                            </Col>
                            <Col lg={2}>
                                <Button>Add Employee</Button>
                            </Col>
                        </Row>
                    </Form>
                </Row><br/>
                
                <Row>
                    <GenericUserTable users={[]}/>   
                </Row>
                          
            </>
    );
}

export default Employee;