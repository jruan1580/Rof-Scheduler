import { Navbar, Nav, Container, NavDropdown} from "react-bootstrap";

function NavigationBar({loginState, handleLoginState}){

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
                {
                    loginState &&
                    <Nav className="ms-auto">
                        <Nav.Item><Nav.Link href="/">Home</Nav.Link></Nav.Item>
                        <NavDropdown title={localStorage.getItem('firstName')}>                        
                            <NavDropdown.Item href="/accountsettings">Account Settings</NavDropdown.Item>
                            <NavDropdown.Item>Logout</NavDropdown.Item>
                        </NavDropdown>  
                    </Nav>                
                }                                                 
            </Container>
        </Navbar>
    );
}

export default NavigationBar;