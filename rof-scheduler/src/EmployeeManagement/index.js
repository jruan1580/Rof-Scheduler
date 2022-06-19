import { useEffect, useState } from 'react';
import { Form, Button, Row, Col, Alert, Pagination } from 'react-bootstrap';
import AddUserModal from '../SharedComponents/AddUser';
import LoadingModal from '../SharedComponents/LoadingModal';
import UpdateUserModal from '../SharedComponents/UpdateUser';
import GenericUserTable from '../SharedComponents/UserTable';
import { getAllEmployees, resetEmployeeLockStatus, updateEmployeeStatus } from '../SharedServices/employeeManagementService';

function EmployeeManagement(){
    //TODO - refactor to use useReducer instead since way too many states
    const [employees, setEmployees] = useState([]);
    const [currPage, setCurrPage] = useState(1);
    const [errorMessage, setErrorMessage] = useState(undefined);
    const [keyword, setKeyword] = useState("");
    const [totalPages, setTotalPages] = useState(0);
    const [showAddModal, setShowAddModal] = useState(false);
    const [showUpdateModal, setShowUpdateModal] = useState(false);
    const [currEmployeeToUpdate, setCurrEmployeeToUpdate] = useState(undefined);
    const [showLoadingModal, setShowLoadingModal] = useState(false);

    useEffect(() =>{        
        (async function(){
            try{
                const employeeByPage = await getAllEmployees(currPage, 10, keyword);
                
                setEmployees(employeeByPage.employees);
                setTotalPages(employeeByPage.totalPages);
            }catch(e){
                setErrorMessage(e.message);                
            }
        })();

    }, [currPage, keyword]);

    const search = (searchEvent) => {
        searchEvent.preventDefault();
        const searchTerm = (searchEvent.target.searchEmployee.value === undefined) ? "" : searchEvent.target.searchEmployee.value;
        setKeyword(searchTerm);
        setCurrPage(1); //reset to current page               
    }

    const reloadAfterThreeSeconds = () => {
        setTimeout(() => window.location.reload(), 3000);
    }

    const resetEmployeeLock = (id) => {
        (async function(){
            try{
                setShowLoadingModal(true);
                await resetEmployeeLockStatus(id);
                for(var i = 0; i < employees.length; i++){
                    if (employees[i].id !== id){
                        continue;
                    }

                    employees[i].isLocked = false;
                }

                setEmployees(employees);
            }catch(e){
                setErrorMessage(e.message);
            }finally{
                setShowLoadingModal(false);
            }
        })();
    }

    const updateEmployeeActiveStatus = (id, status) =>{
        (async function(){
            try{
                setShowLoadingModal(true);
                await updateEmployeeStatus(id, status);
                for(var i = 0; i < employees.length; i++){
                    if (employees[i].id !== id){
                        continue;
                    }

                    employees[i].active = status;
                }

                setEmployees(employees);
            }catch(e){
                setErrorMessage(e.message);
            }finally{
                setShowLoadingModal(false);
            }
        })();
    }
    
    const loadUpdateModal = (user) =>{
        setCurrEmployeeToUpdate(user);

        setShowUpdateModal(true);
    }

    const closeUpdateModal = () =>{
        setCurrEmployeeToUpdate(undefined);

        setShowUpdateModal(false);
    }

    return(
            <>
                <h1>Employee Management</h1><br/>
                <AddUserModal userType='Employee' show={showAddModal} handleHide={() => setShowAddModal(false)} handleUserAddSuccess={reloadAfterThreeSeconds}/>
                <UpdateUserModal user={currEmployeeToUpdate} userType='Employee' show={showUpdateModal} hideModal={closeUpdateModal}/>
                <LoadingModal show={showLoadingModal}/>
                <Row>
                    <Form  onSubmit={search}>
                        <Row className="align-items-center">
                            <Col lg={4}>            
                                <Form.Control name="searchEmployee" id="searchEmployee" placeholder="Search employee by name, email..etc." />
                            </Col>
                            <Col lg={6}>
                                <Button type="submit">Search</Button>
                            </Col>
                            <Col lg={2}>
                                <Button onClick={() => setShowAddModal(true)}>Add Employee</Button>
                            </Col>
                        </Row>
                    </Form>
                </Row><br/>
                {errorMessage !== undefined && <Alert variant='danger'>{errorMessage}</Alert>}
                <Row>
                    <GenericUserTable users={employees} resetEmployeeLockStatus={resetEmployeeLock} updateEmployeeActiveStatus={updateEmployeeActiveStatus} showUpdateModal={loadUpdateModal}/>   
                </Row>
                <Pagination>
                    {currPage != 1 && <Pagination.Prev onClick={() => setCurrPage(currPage - 1)}/> }
                    {currPage != totalPages && <Pagination.Next onClick={() => setCurrPage(currPage + 1)}/> }
                </Pagination>
                          
            </>
    );
}

export default EmployeeManagement;