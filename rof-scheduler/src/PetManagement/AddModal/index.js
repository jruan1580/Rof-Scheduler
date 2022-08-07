import { useEffect, useState } from "react";
import { Modal, Form, Row, Col, Button } from "react-bootstrap";
import Select from 'react-select';

import { getPetTypes, getBreedByPetType, getVaccinesByPetType } from "../../SharedServices/dropdownService";

function AddPetModal({show, closeModal, setLoginState }){
    const [petTypes, setPetTypes] = useState([]);
    const [petTypeSelected, setPetTypeSelected] = useState(undefined);
    const [breedByPetType, setBreedByPetType] = useState([]);
    const [vaccinesByPetType, setVaccinesByPetType] = useState([]);
      
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

            }catch(e){

            }
        })();

        //this will hide pet type ddl and unhide add pet modal
        setPetTypeSelected(petTypeIdSelected);
    }
    
    const constructBreedOptions = (breeds) =>{
        const breedOptions = [];
        for(var i = 0; i < breeds.length; i++){
            breedOptions.push({ label: breeds[i].vaccineName, value: breeds[i].id });
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
            const vax = { vaxId: vaccines[i].id, vaxName: vaccines[i].vaccineName, checked: false }; //since its new, checked is false
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

    const setVaccineValue = (colIndex, vaccineIndex) =>{
        //value equals opposite of what it currently is
        ((vaccinesByPetType[colIndex])[vaccineIndex]).checked = !((vaccinesByPetType[colIndex])[vaccineIndex]).checked;
        setVaccinesByPetType(vaccinesByPetType);
    }

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
                        <Form onSubmit={goToAddPet}>
                            <Form.Group as={Row}>
                                <Form.Label column lg={3} >Pet Type:</Form.Label>
                                <Col lg={9}>
                                    <Form.Select
                                        type="select"
                                        placeholder="petType"
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
                                        options={breedByPetType}
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