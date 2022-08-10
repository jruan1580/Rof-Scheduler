import { useEffect, useState } from "react";
import { Modal, Form, Row, Col, Button, Spinner, Alert } from "react-bootstrap";
import Select from 'react-select';

import { getPetTypes, getBreedByPetType, getVaccinesByPetType, getClients } from "../../SharedServices/dropdownService";
import { ensureAddPetInformationProvided } from "../../SharedServices/inputValidationService";
import { addPet } from "../../SharedServices/petManagementService";
import "./addPet.css";

function AddPetModal({show, handleHide, setLoginState }){
    //TODO - many states here, switch to useReduce later
    const [petTypes, setPetTypes] = useState([]);
    const [petTypeSelected, setPetTypeSelected] = useState(undefined);
    const [breedByPetType, setBreedByPetType] = useState([]);
    const [vaccinesByPetType, setVaccinesByPetType] = useState([]);
    const [owners, setOwners] = useState([]);
    const [validationMap, setValidationMap] = useState(new Map());
    const [errMsg, setErrMsg] = useState(undefined);
    const [successMsg, setSuccessMsg] = useState(false);
    const [loading, setLoading] = useState(false);
    const [disableBtns, setDisableBtns] = useState(false);

    //load pet types when we land on page
    useEffect(() =>{
        (async function(){
            try{
                const resp = await getPetTypes();
                if (resp.status === 401){
                    setLoginState(false);
                    return;
                }

                const petTypes = await resp.json();
                setPetTypes(petTypes);
            }catch(e){

            }
        })();

    }, []);

    const goToAddPet = (e) =>{
        e.preventDefault();

        const petTypeIdSelected = parseInt(e.target.petType.value);

        /*
         * get breeds by pet type.
         * get vaccines by pet type.
         * 
         * depending on use role, we may or may not need to grab list of clients.
         * if role == client, that means that owner is the client currently logged on.
         * if role == employee or admin, they are adding pet for a client. so we will need to grab a list of clients.
         * then hide pet type ddl and go to (unhide) add pet modal
         */
        (async function(){
            try{
                //grab breeds by pet type selected
                var resp = await getBreedByPetType(petTypeIdSelected);
                if (resp.status === 401){
                    setLoginState(false);
                    return;
                }

                const breeds = await resp.json();
                constructBreedOptions(breeds);
                
                //grab vaccines by pet type selected
                resp = await getVaccinesByPetType(petTypeIdSelected);
                if (resp.status === 401){
                    setLoginState(false);
                    return;
                }

                const vaccines = await resp.json();
                constructVaccinesByPetType(vaccines);

                //employee or admin, need to get a list of clients
                if (localStorage.getItem("role").toLowerCase() !== "client"){
                    resp = await getClients();
                    if (resp.status === 401){
                        setLoginState(false);
                        return;
                    }

                    const clients = await resp.json();
                    constructClientsOption(clients);
                }

                //this will hide pet type ddl and unhide add pet modal
                setErrMsg(undefined);
                setPetTypeSelected(petTypeIdSelected);
            }catch(e){
                setErrMsg(e.message);
                return;
            }
        })();        
    }
    
    const constructBreedOptions = (breeds) =>{
        const breedOptions = [];
        for(var i = 0; i < breeds.length; i++){
            breedOptions.push({ value: breeds[i].id, label: breeds[i].breedName });
        }

        setBreedByPetType(breedOptions);
    }

    const constructVaccinesByPetType = (vaccines) => {
        //on the ui, we are going to break vaccines up into 4 columns
        //so we will evenly split vaccines up into group of 4
        const vaccinesByCol = [];
        //push 4 empty lists first represent 4 columns of list of vaccines
        vaccinesByCol.push([]);
        vaccinesByCol.push([]);
        vaccinesByCol.push([]);
        vaccinesByCol.push([]);

        //loop through vaccine and push each vaccine into each column.
        //when we hit last column, then reset back to first column
        var col = 0; //start at first column
        for(var i = 0; i < vaccines.length; i++){
            const vax = { id: vaccines[i].id, vaxName: vaccines[i].vaccineName, checked: false }; //since its new, checked is false
            vaccinesByCol[col].push(vax);

            col++; //go to next column
            if (col == 4){
                //when col == 3, that means we were at 4th column already (zero indexed). 
                //col ++ will increment to 4 meaning we need to reset back to fist column
                col = 0; 
            }
        }   
        
        setVaccinesByPetType(vaccinesByCol);
    }

    const constructClientsOption = (clients) =>{
        const clientOptions = [];
        for(var i = 0; i < clients.length; i++){
            clientOptions.push({ label: clients[i].fullName, value: clients[i].id });
        }

        setOwners(clientOptions);
    }

    const setVaccineValue = (colIndex, vaccineIndex) =>{
        //value equals opposite of what it currently is
        ((vaccinesByPetType[colIndex])[vaccineIndex]).checked = !((vaccinesByPetType[colIndex])[vaccineIndex]).checked;
        setVaccinesByPetType(vaccinesByPetType);
    }

    const addPetSubmit = (e) =>{
        e.preventDefault();

        setErrMsg(undefined);

        const petName = e.target.petName.value;
        const breed = parseInt(e.target.breed.value);
        const dob = e.target.dob.value;
        const weight = parseFloat(e.target.weight.value);
        const otherInfo = e.target.additionalInfo.value;

        var client = undefined;
        if (localStorage.getItem("role").toLowerCase() != "client"){
            client = parseInt(e.target.client.value);
        }

        var inputValidations = ensureAddPetInformationProvided(petName, breed, weight, dob, client);
        console.log(inputValidations);
        if (inputValidations.size > 0){
            setValidationMap(inputValidations);
            return;
        }

        setValidationMap(new Map());
        setLoading(true);

        const vaccineStatus = [];
        for(var row = 0; row < vaccinesByPetType.length; row++){
            const vaxRow = vaccinesByPetType[row];
            for(var col = 0; col < vaxRow.length; col++){
                vaccineStatus.push({ id: vaxRow[col].id, vaccineName: vaxRow[col].vaxName, innoculated: vaxRow[col].checked });
            }
        }

        (async function(){
            try{
                //if current user is client, get id from local storage.
                //else its the selected client from dropdown list
                const ownerId = (localStorage.getItem("role").toLowerCase() !== "client") ? client : parseInt(localStorage.getItem("id"));
                const resp = await addPet(ownerId, petTypeSelected, breed, petName, dob, weight, otherInfo, vaccineStatus);

                if (resp !== undefined && resp.status === 401){
                    setLoginState(false);
                    return;
                }
    
                setErrMsg(undefined);
                setDisableBtns(true);
    
                setSuccessMsg(true);
            }catch(e){
                setErrMsg(e.message);
                return;
            }finally{
                setLoading(false);
            }
        })();
    }

    //reset everything when we close modal
    const closeModal = function () {
        setValidationMap(new Map());
        setErrMsg(undefined);
        setPetTypeSelected(undefined);
        handleHide();
      };

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
                        <Form onSubmit={goToAddPet}>
                            <Form.Group as={Row}>
                                <Form.Label column lg={3} >Pet Type:</Form.Label>
                                <Col lg={9}>
                                    <Form.Select
                                        type="select"
                                        name="petType"
                                    >
                                        {
                                            petTypes.map((petType) =>{
                                                return(
                                                    <option key={petType.id} value={petType.id}>{petType.petTypeName}</option>
                                                )
                                            })
                                        }                                      
                                    </Form.Select>
                                </Col>                                             
                            </Form.Group>
                            <br />
                            <hr></hr>
                            <Button type="submit" className="float-end">
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
                        <Form onSubmit={addPetSubmit}>
                            {errMsg !== undefined && <Alert variant="danger">{errMsg}</Alert>}
                            {
                                successMsg &&                         
                                <Alert variant="success">
                                    Pet successfully added! Page will reload in 3 seconds and new pet will be available....
                                </Alert>
                            }

                            <h4>Pet Information</h4>
                            <br />

                            <Row>
                                <Form.Group as={Col} lg={3}>
                                    <Form.Label>Name</Form.Label>
                                    <Form.Control 
                                        placeholder="Pet Name"
                                        name="petName"
                                        isInvalid={validationMap.has("petName")}
                                    />
                                    <Form.Control.Feedback type="invalid">
                                        {validationMap.get("petName")}
                                    </Form.Control.Feedback>
                                </Form.Group>
                                
                                <Form.Group as={Col} lg={3}>
                                    <Form.Label>Breed</Form.Label>
                                    {/* <Form.Select
                                        type="select"
                                        name="breed"
                                        isInvalid={validationMap.has("breed")}
                                    >
                                        {
                                            breedByPetType.map((breed) =>{
                                                return(
                                                    <option key={breed.id} value={breed.id}>{breed.breedName}</option>
                                                )
                                            })
                                        }                                      
                                    </Form.Select> */}
                                    <Select
                                        name="breed"
                                        options={breedByPetType}
                                        defaultValue={{ label: "Select Breed", value: 0 }}
                                        isInvalid={validationMap.has("breed")}
                                        style={{borderColor:'red'}}
                                    /> 
                                    <div className="dropdown-invalid"> {validationMap.get("breed")}</div>
                                    {/* <Form.Control.Feedback type="invalid">
                                        {validationMap.get("breed")}
                                    </Form.Control.Feedback>                             */}
                                </Form.Group>

                                <Form.Group as={Col} lg={3}>
                                    <Form.Label>DOB</Form.Label>
                                    <Form.Control 
                                        type="date"
                                        placeholder="DOB"
                                        name="dob"
                                        isInvalid={validationMap.has("dob")}
                                    />
                                    <Form.Control.Feedback type="invalid">
                                        {validationMap.get("dob")}
                                    </Form.Control.Feedback>  
                                </Form.Group>

                                <Form.Group as={Col} lg={3}>
                                    <Form.Label>Weight</Form.Label>
                                    <Form.Control 
                                        type="number"
                                        placeholder="weight"
                                        name="weight"
                                        isInvalid={validationMap.has("weight")}
                                    />
                                    <Form.Control.Feedback type="invalid">
                                        {validationMap.get("weight")}
                                    </Form.Control.Feedback> 
                                </Form.Group>
                            </Row><br/>
                            {
                                localStorage.getItem("role").toLowerCase() !== "client" &&
                                <>
                                    <Row>
                                        <Form.Group as={Col} lg={4}>
                                            <Form.Label>Select Owner</Form.Label>
                                            <Select
                                                name="client"
                                                defaultValue={{label: "Select Owner", value:0}}
                                                options={owners}
                                            />        
                                            <div className="dropdown-invalid"> {validationMap.get("client")}</div>
                                                                         
                                        </Form.Group>
                                    </Row><br/>
                                </>                                
                            }
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
                                {
                                    //first column
                                    vaccinesByPetType[0].map((vaccine, index) =>{
                                        return(
                                            <Form.Check
                                                key={vaccine.id}
                                                type="checkbox"
                                                label={vaccine.vaxName}
                                                value={vaccine.checked}
                                                onChange={() => setVaccineValue(0, index)}//first param tells us which column, second param tells us which index value to update
                                            />
                                        )
                                    })
                                }                                
                                </Form.Group>
                                <Form.Group as={Col} lg={3}>
                                {
                                    //second column
                                    vaccinesByPetType[1].map((vaccine, index) =>{
                                        return(
                                            <Form.Check
                                                key={vaccine.id}
                                                type="checkbox"
                                                label={vaccine.vaxName}
                                                value={vaccine.checked}
                                                onChange={() => setVaccineValue(1, index)} //first param tells us which column, second param tells us which index value to update
                                            />
                                        )
                                    })
                                }     
                                </Form.Group>
                                <Form.Group as={Col} lg={3}>
                                {
                                    //third column
                                    vaccinesByPetType[2].map((vaccine, index) =>{
                                        return(
                                            <Form.Check
                                                key={vaccine.id}
                                                type="checkbox"
                                                label={vaccine.vaxName}
                                                value={vaccine.checked}
                                                onChange={() => setVaccineValue(2, index)}//first param tells us which column, second param tells us which index value to update
                                            />
                                        )
                                    })
                                }     
                                </Form.Group>
                                <Form.Group as={Col} lg={3}>
                                {
                                    //fourth column
                                    vaccinesByPetType[3].map((vaccine, index) =>{
                                        return(
                                            <Form.Check
                                                key={vaccine.id}
                                                type="checkbox"
                                                label={vaccine.vaxName}
                                                value={vaccine.checked}
                                                onChange={() => setVaccineValue(3, index)}//first param tells us which column, second param tells us which index value to update
                                            />
                                        )
                                    })
                                }         
                                </Form.Group>
                                
                            </Row>

                            <br />
                            <hr></hr>
                            {(loading || disableBtns) && (
                                <Button
                                    type="button"
                                    variant="danger"
                                    className="float-end ms-2"
                                    disabled
                                >
                                    Cancel
                                </Button>
                            )}
                            {!loading && !disableBtns && (
                                <Button
                                    type="button"
                                    variant="danger"
                                    onClick={() => closeModal()}
                                    className="float-end ms-2"
                                >
                                    Cancel
                                </Button>
                            )}
                            {(loading || disableBtns) && (
                                <Button variant="primary" className="float-end" disabled>
                                    <Spinner
                                        as="span"
                                        animation="grow"
                                        size="sm"
                                        role="status"
                                        aria-hidden="true"
                                    />
                                    Loading...
                                </Button>
                            )}
                            {!loading && !disableBtns && (
                                <Button type="submit" className="float-end">
                                    Create
                                </Button>
                            )}
                        </Form>
                        
                    </Modal.Body>
                </Modal>
            }
            
        </>
    )
}

export default AddPetModal;