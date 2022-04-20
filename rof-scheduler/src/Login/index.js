import { Form, Button, Row, Col, Spinner, Card, Container } from 'react-bootstrap';
import { useState } from 'react';
import { validateLoginPassword } from '../Services/inputValidationService';
import { login } from '../Services/employeeManagementService';

function Login(){
    const [loading, setLoading] = useState(false);
    const [passwordErrMsg, setPasswordErrMsg] = useState('');
    const [usernameError, setUsernameError] = useState(false);

    const handleSubmit = (submitEvent) => {
        //prevents reload when submit
        submitEvent.preventDefault();
        
        const username = submitEvent.target.username.value;
        const password = submitEvent.target.password.value;
        const passwordValidation = validateLoginPassword(password);

        if (passwordValidation !== '' || username === undefined) {
            setPasswordErrMsg(passwordValidation);
            if (username === undefined || username === ''){
                setUsernameError(true);
            }else{
                setUsernameError(false);
            }
            submitEvent.stopPropagation();
            
        }else{
            setLoading(true);    
            (async function(){
                try{
                    const resp = await login(username, password);

                    
                }catch(e){
                    alert('Failed to add movie with error: ' + e.message);
                }finally{
                    setLoading(false);
                }     
            })();
        }       
    }

    return(
        <>           
            <Container>
                <br />
                <Row>
                    <Col lg="4"></Col>
                    <Col lg="4">
                        <Card border="dark">
                            <Card.Header>Login</Card.Header>
                            <Card.Body>
                                <Form noValidate onSubmit={handleSubmit}>
                                    <Form.Group className="mb-3">
                                        <Form.Label>Username</Form.Label>
                                        <Form.Control type="text" placeholder="Enter username" name="username" isInvalid={usernameError}/>   
                                        <Form.Control.Feedback type="invalid">
                                            Please enter a username.
                                        </Form.Control.Feedback>    
                                    </Form.Group>

                                    <Form.Group className="mb-3">
                                        <Form.Label>Password</Form.Label>
                                        <Form.Control type="password" placeholder="Password" name="password" isInvalid ={passwordErrMsg !== ''} required/>
                                        <Form.Control.Feedback type="invalid">
                                            {passwordErrMsg}
                                        </Form.Control.Feedback>    
                                    </Form.Group>    
                                    
                                    {
                                        loading && 
                                        <Button variant="primary" disabled>
                                            <Spinner
                                                as="span"
                                                animation="grow"
                                                size="sm"
                                                role="status"
                                                aria-hidden="true"
                                            />
                                            Loading...
                                        </Button> 
                                    }                        
                                    {
                                        !loading && 
                                        <Button variant="primary" type="submit">Submit</Button> 
                                    }                                              
                                </Form>
                            </Card.Body>
                        </Card>
                        
                    </Col>
                    <Col lg="4"></Col>
                </Row>    
            </Container>                   
        </>
    );
}

export default Login;