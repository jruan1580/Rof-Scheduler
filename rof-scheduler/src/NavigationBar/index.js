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
                        <NavDropdown title='User Management'>         
                            <NavDropdown.Item as={Link} to="/employeemanagement">Employee</NavDropdown.Item>
                            <NavDropdown.Item as={Link} to="/clientmanagement">Client</NavDropdown.Item>
                        </NavDropdown>  
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