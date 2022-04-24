import Navbar from "react-bootstrap/Navbar";
import Nav from "react-bootstrap/Nav";
import Container from "react-bootstrap/Container";

function NavigationBar({loginState, handleLoginState}){
    return(
        <Navbar style={{"backgroundColor":"#AEAEAE"}} variant="light">
            <Container>
                <Navbar.Brand href="#">
                    <img
                        src="./rof_logo.png"
                        width="185px"
                        className="align-top"
                    />
                </Navbar.Brand>
                {
                    loginState &&
                    <Nav className="ms-auto">
                        <Nav.Link href="#">Home</Nav.Link>
                    </Nav>                
                }                
                {/* <Navbar.Brand href="#">
                    Rof Scheduler
                </Navbar.Brand> */}
                
            </Container>
        </Navbar>
    );
}

export default NavigationBar;