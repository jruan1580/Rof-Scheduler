import { useState, useEffect } from "react";
import {
  Form,
  Button,
  Row,
  Col,
  OverlayTrigger,
  Table,
  Tooltip,
  Alert,
} from "react-bootstrap";

import TablePagination from "../SharedComponents/TablePagination";

import{
    getAllPetServices,
    deletePetService
} from "../SharedServices/petServiceManagementService";

import AddPetService from "./AddPetService";
import UpdatePetService from "./UpdatePetService";


function PetService({ setLoginState }){
    const [errMsg, setErrMsg] = useState(undefined);
    const [petServices, setPetServices] = useState([]);
    const [currPage, setCurrPage] = useState(1);
    const [totalPages, setTotalPages] = useState(0);
    const [searchKeyword, setSearchKeyword] = useState("");
    const [showAddModal, setShowAddModal] = useState(false);
    const [showUpdateModal, setShowUpdateModal] = useState(false);
    const [currPetServiceToUpdate, setCurrPetServiceToUpdate] = useState(undefined);

    useEffect(() =>{
        (async function () {
            try {
                var resp = await getAllPetServices(currPage, 10, searchKeyword);
                
                if (resp.status === 401) {
                    setLoginState(false);
                    return;
                }
                
                const petServicesWithTotalPages = await resp.json();

                setPetServices(petServicesWithTotalPages.petServices);
                setTotalPages(petServicesWithTotalPages.totalPages);                
            } catch (e) {
              setErrMsg(e.message);
            }
          })();
    }, [currPage, searchKeyword]);

    const search = (searchEvent) => {
        searchEvent.preventDefault();
    
        const searchTerm = searchEvent.target.searchPetService.value === undefined
          ? ""
          : searchEvent.target.searchPetService.value;

          setSearchKeyword(searchTerm);
        setCurrPage(1);
    };

    const reloadAfterThreeSeconds = () => {
        setTimeout(() => window.location.reload(), 3000);
    };

    const loadUpdateModal = (petServiceToUpdate) => {
        setCurrPetServiceToUpdate(petServiceToUpdate);
    
        setShowUpdateModal(true);
    };

    const postUpdatePetServiceAction = function(updatedFieldsMap){
        const id = updatedFieldsMap.get('id');
        const indexOfPetService = petServices.findIndex((petService) => petService.id == id);

        petServices[indexOfPetService].name = updatedFieldsMap.get('name');
        petServices[indexOfPetService].rate = updatedFieldsMap.get('rate');
        petServices[indexOfPetService].employeeRate = updatedFieldsMap.get('employeeRate');
        petServices[indexOfPetService].description = updatedFieldsMap.get('description');
        petServices[indexOfPetService].duration = updatedFieldsMap.get('duration');
        petServices[indexOfPetService].timeUnit = updatedFieldsMap.get('timeUnit');

        setPetServices(petServices);
    }

    const removePetService = function(id){
        (async function(){
            try{
                var resp = await deletePetService(id);

                if (resp.status === 401) {
                    setLoginState(false);
                    return;
                }

                var pageToRequery = currPage; //set default page to requery to curr page
                //curr page has one pet service and we just deleted it and we still have a previous page
                if (petServices.length === 1 && currPage !== 1){
                    pageToRequery = currPage - 1; //query previous page
                }   
                
                //get pet services for current page again to reload after delete
                resp = await getAllPetServices(pageToRequery, 10, searchKeyword);
                if (resp.status === 401) {
                    setLoginState(false);
                    return;
                }

                const petServicesWithTotalPages = await resp.json();
              
                setPetServices(petServicesWithTotalPages.petServices);
                setTotalPages(petServicesWithTotalPages.totalPages);  
                if (pageToRequery != currPage)  {
                    setCurrPage(pageToRequery);
                }
                
            }catch(e){
                setErrMsg('Failed to delete with error: ' + e.message);
            }                
        })();
    }

    return(
        <>
            <AddPetService 
                show={showAddModal}
                handleHide={() => setShowAddModal(false)}
                setLoginState={setLoginState}
                reloadAfterThreeSeconds={reloadAfterThreeSeconds}
            />

            <UpdatePetService
                 petService={currPetServiceToUpdate}
                 show={showUpdateModal}
                 hide={() => setShowUpdateModal(false)}
                 setLoginState={setLoginState}
                 postUpdatePetAction={postUpdatePetServiceAction}
            />
            
            <Row>
                <Form onSubmit={search}>
                    <Row className="align-items-center">
                        <Col lg={4}>
                            <Form.Control
                                name="searchPetService"
                                id="searchPetService"
                                placeholder="Search pet service by name or description keyword"
                            />
                        </Col>
                        <Col lg={6}>
                            <Button type="submit">Search</Button>
                        </Col>
                        <Col lg={2}>
                            <Button onClick={() => setShowAddModal(true)}>Add Service</Button>
                        </Col>
                    </Row>
                </Form>
            </Row>
            <br />
            {errMsg !== undefined && <Alert variant="danger">{errMsg}</Alert>}
            <Row>
                <Table responsive striped bordered>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Service's Rate</th>                    
                            <th>Employee's Pay Rate (%)</th>                            
                            <th>Duration</th>
                            <th>Description</th>                            
                            <th colSpan={2}></th>
                        </tr>
                    </thead>    
                    <tbody>
                        {
                            petServices.length != 0 &&
                            petServices.map((petService) => {
                                return(
                                    <tr key={petService.id}>
                                        <td>{petService.name}</td>
                                        <td>{petService.rate}</td>
                                        <td>{petService.employeeRate}</td>
                                        <td>{petService.duration} {petService.timeUnit}</td>
                                        <td>{petService.description}</td>
                                        <td>
                                            <OverlayTrigger
                                                placement="top"
                                                overlay={<Tooltip>Update</Tooltip>}
                                            >
                                                <Button onClick={() => loadUpdateModal(petService)}>
                                                    <svg
                                                        xmlns="http://www.w3.org/2000/svg"
                                                        width="16"
                                                        height="16"
                                                        fill="currentColor"
                                                        className="bi bi-pencil-fill"
                                                        viewBox="0 0 16 16"
                                                    >
                                                        <path d="M12.854.146a.5.5 0 0 0-.707 0L10.5 1.793 14.207 5.5l1.647-1.646a.5.5 0 0 0 0-.708l-3-3zm.646 6.061L9.793 2.5 3.293 9H3.5a.5.5 0 0 1 .5.5v.5h.5a.5.5 0 0 1 .5.5v.5h.5a.5.5 0 0 1 .5.5v.5h.5a.5.5 0 0 1 .5.5v.207l6.5-6.5zm-7.468 7.468A.5.5 0 0 1 6 13.5V13h-.5a.5.5 0 0 1-.5-.5V12h-.5a.5.5 0 0 1-.5-.5V11h-.5a.5.5 0 0 1-.5-.5V10h-.5a.499.499 0 0 1-.175-.032l-.179.178a.5.5 0 0 0-.11.168l-2 5a.5.5 0 0 0 .65.65l5-2a.5.5 0 0 0 .168-.11l.178-.178z" />
                                                    </svg>
                                                </Button>
                                            </OverlayTrigger>
                                        </td>
                                        <td>
                                            <OverlayTrigger
                                                placement="top"
                                                overlay={<Tooltip>Delete</Tooltip>}
                                            >
                                                <Button onClick={() => removePetService(petService.id)}>
                                                    <svg
                                                        xmlns="http://www.w3.org/2000/svg"
                                                        width="16"
                                                        height="16"
                                                        fill="currentColor"
                                                        className="bi bi-trash3"
                                                        viewBox="0 0 16 16"                                                        
                                                    >
                                                        <path d="M6.5 1h3a.5.5 0 0 1 .5.5v1H6v-1a.5.5 0 0 1 .5-.5ZM11 2.5v-1A1.5 1.5 0 0 0 9.5 0h-3A1.5 1.5 0 0 0 5 1.5v1H2.506a.58.58 0 0 0-.01 0H1.5a.5.5 0 0 0 0 1h.538l.853 10.66A2 2 0 0 0 4.885 16h6.23a2 2 0 0 0 1.994-1.84l.853-10.66h.538a.5.5 0 0 0 0-1h-.995a.59.59 0 0 0-.01 0H11Zm1.958 1-.846 10.58a1 1 0 0 1-.997.92h-6.23a1 1 0 0 1-.997-.92L3.042 3.5h9.916Zm-7.487 1a.5.5 0 0 1 .528.47l.5 8.5a.5.5 0 0 1-.998.06L5 5.03a.5.5 0 0 1 .47-.53Zm5.058 0a.5.5 0 0 1 .47.53l-.5 8.5a.5.5 0 1 1-.998-.06l.5-8.5a.5.5 0 0 1 .528-.47ZM8 4.5a.5.5 0 0 1 .5.5v8.5a.5.5 0 0 1-1 0V5a.5.5 0 0 1 .5-.5Z" />
                                                    </svg>
                                                </Button>
                                            </OverlayTrigger>
                                        </td>
                                    </tr>
                                );
                            })
                        }      

                        {petServices.length == 0 && (
                            <tr>
                                <td colSpan={6} style={{ textAlign: "center" }}>
                                    No services available. Please add a service.
                                </td>
                            </tr>
                        )}                  
                    </tbody>
                </Table>
            </Row>
            
            <TablePagination currPage={currPage} totalPages={totalPages} setCurrPage={setCurrPage} />
        </>
    );
}

export default PetService;