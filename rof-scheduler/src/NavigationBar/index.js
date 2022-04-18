import Navbar from "react-bootstrap/Navbar";
import Nav from "react-bootstrap/Nav";
import Container from "react-bootstrap/Container";

function NavigationBar(){
    return(
        <Navbar bg="dark" variant="dark">
            <Container>
                <Navbar.Brand href="#">Rof Scheduler</Navbar.Brand>
                {/* <Nav className="me-auto">
                    <Nav.Link href="#">Home</Nav.Link>
                </Nav> */}
            </Container>
        </Navbar>
    );
}

export default NavigationBar;