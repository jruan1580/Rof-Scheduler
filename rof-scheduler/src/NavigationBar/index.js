import { Navbar, Nav, Container, NavDropdown} from "react-bootstrap";
import { Link } from "react-router-dom";
import { logoff } from '../SharedServices/authenticationService';

function NavigationBar({loginState, handleLoginState}){

    const logout = () => {
        (async function(){
            try{
                await logoff();                
                localStorage.clear();                
                handleLoginState(false);

                window.location.href = "/";
            }catch(e){
                alert('Failed to logoff... Try Again');
                console.log(e.message);
            }
        })();
    }
    return(
        <Navbar style={{"backgroundColor":"#AEAEAE"}} variant="light">
            <Container>
                <Navbar.Brand href="/">
                    <img
                        src="./rof_logo.png"
                        width="185px"
                        className="align-top"
                    />
                </Navbar.Brand> 
                <Nav className="ms-auto">
                    {loginState && <Nav.Item><Nav.Link href="/">Home</Nav.Link></Nav.Item>}
                    {
                        loginState && localStorage.getItem('role') === 'Administrator' &&
                        <>
                            <Nav.Item><Nav.Link href="/employeemanagement">Employees</Nav.Link></Nav.Item>
                            <NavDropdown title='Client Management'>         
                                <NavDropdown.Item as={Link} to="/clientmanagement">Client</NavDropdown.Item>
                                <NavDropdown.Item as={Link} to="/petmanagement">Pet</NavDropdown.Item>
                            </NavDropdown>
                            <NavDropdown title='Holiday Management'>
                                <NavDropdown.Item as={Link} to="/holidaymanagement">Holidays</NavDropdown.Item>
                            </NavDropdown>
                            <Nav.Item><Nav.Link href="/petservicemanagement">Pet Services</Nav.Link></Nav.Item>
                            <NavDropdown title='Summary'>         
                                <NavDropdown.Item as={Link} to="/revenue">Revenue</NavDropdown.Item>
                            </NavDropdown>
                        </>                        
                    }
                    {
                        loginState && localStorage.getItem('role') === 'Employee' &&
                        <NavDropdown title='Client Management'>         
                            <NavDropdown.Item as={Link} to="/clientmanagement">Client</NavDropdown.Item>
                            <NavDropdown.Item as={Link} to="/petmanagement">Pet</NavDropdown.Item>
                        </NavDropdown>  
                    }
                    {
                        loginState && localStorage.getItem('role') === 'Client' &&
                        <Nav.Item><Nav.Link href="/petmanagement">Pets</Nav.Link></Nav.Item>
                    }
                    {
                        loginState &&
                        <NavDropdown title={localStorage.getItem('firstName')}>                        
                            <NavDropdown.Item as={Link} to="/accountsettings">Account Settings</NavDropdown.Item>
                            <NavDropdown.Item onClick={logout}>Logout</NavDropdown.Item>
                        </NavDropdown>  
                    }
                </Nav> 
                                                                     
            </Container>
        </Navbar>
    );
}

export default NavigationBar;