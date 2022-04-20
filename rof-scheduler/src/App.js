import NavigationBar from './NavigationBar';
import Login from './Login';
import { Container } from 'react-bootstrap';

function App() {
  return (
    <>
      <NavigationBar />
      <Container>
        <Login/>     
      </Container>      
    </>
  );
}

export default App;
