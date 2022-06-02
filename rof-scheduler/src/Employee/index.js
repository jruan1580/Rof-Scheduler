import { useEffect, useState } from 'react';
import { Form, Button, Row, Col, Alert, Pagination } from 'react-bootstrap';
import GenericUserTable from '../SharedComponents/UserTable';
import { getAllEmployees } from '../SharedServices/employeeManagementService';

function Employee(){
    const [employees, setEmployees] = useState([]);
    const [currPage, setCurrPage] = useState(1);
    const [errorMessage, setErrorMessage] = useState(undefined);

    useEffect(() =>{
        (async function(){
            try{
                const employeeByPage = await getAllEmployees(currPage, 10);
                
                setEmployees(employeeByPage);
            }catch(e){
                setErrorMessage(e.message);                
            }
        })();

    }, [currPage]);

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
                {errorMessage !== undefined && <Alert variant='danger'>{errorMessage}</Alert>}
                <Row>
                    <GenericUserTable users={employees}/>   
                </Row>
                <Pagination>
                    {currPage != 1 && <Pagination.Prev /> }
                    {employees.length == 10 && <Pagination.Next /> }
                </Pagination>
                          
            </>
    );
}

export default Employee;