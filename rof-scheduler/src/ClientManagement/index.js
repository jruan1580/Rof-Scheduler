import { useEffect, useState } from 'react';
import { Form, Button, Row, Col, Alert, Pagination } from 'react-bootstrap';
import AddUserModal from '../SharedComponents/AddUser';
import GenericUserTable from '../SharedComponents/UserTable';
import { getAllClients } from '../SharedServices/clientManagementService';

function ClientManagement(){
    const [clients, setClients] = useState([]);
    const [currPage, setCurrPage] = useState(1);
    const [errorMessage, setErrorMessage] = useState(undefined);
    const [keyword, setKeyword] = useState("");
    const [totalPages, setTotalPages] = useState(0);
    const [showAddModal, setShowAddModal] = useState(false);

    useEffect(() =>{        
        (async function(){
            try{
                const clientByPage = await getAllClients(currPage, 10, keyword);
                
                setClients(clientByPage.clients);
                setTotalPages(clientByPage.totalPages);
            }catch(e){
                setErrorMessage(e.message);                
            }
        })();

    }, [currPage, keyword]);

    const search = (searchEvent) => {
        searchEvent.preventDefault();
        const searchTerm = (searchEvent.target.searchClient.value === undefined) ? "" : searchEvent.target.searchClient.value;
        setKeyword(searchTerm);
        setCurrPage(1);              
    }

    const reloadAfterThreeSeconds = () => {
        setTimeout(() => window.location.reload(), 3000);
    }

    return(
            <>
                <h1>Client Management</h1><br/>
                <AddUserModal userType='Client' show={showAddModal} handleHide={() => setShowAddModal(false)} handleUserAddSuccess={reloadAfterThreeSeconds}/>
                <Row>
                    <Form  onSubmit={search}>
                        <Row className="align-items-center">
                            <Col lg={4}>            
                                <Form.Control name="searchClient" id="searchClient" placeholder="Search client by name, email..etc." />
                            </Col>
                            <Col lg={6}>
                                <Button type="submit">Search</Button>
                            </Col>
                            <Col lg={2}>
                                <Button onClick={() => setShowAddModal(true)}>Add Client</Button>
                            </Col>
                        </Row>
                    </Form>
                </Row><br/>
                {errorMessage !== undefined && <Alert variant='danger'>{errorMessage}</Alert>}
                <Row>
                    <GenericUserTable users={clients}/>   
                </Row>
                <Pagination>
                    {currPage != 1 && <Pagination.Prev onClick={() => setCurrPage(currPage - 1)}/> }
                    {currPage != totalPages && <Pagination.Next onClick={() => setCurrPage(currPage + 1)}/> }
                </Pagination>
                          
            </>
    );
}

export default ClientManagement;