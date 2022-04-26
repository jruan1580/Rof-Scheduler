import NavigationBar from './NavigationBar';
import Login from './Login';
import { Container } from 'react-bootstrap';
import { useEffect, useState } from 'react';
import FullCalendar from './FullCalendar';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
function App() {
  const [login, setLogin] = useState(false);

  useEffect(() => {
    console.log(localStorage.getItem("Id"));

    if (localStorage.getItem("Id") != undefined){
      setLogin(true);
    }
  }, []);

  return (
    <>
      <BrowserRouter>
        <NavigationBar loginState={login} handleLoginState={setLogin}/>
        <Container>
          <Routes>
            {!login &&  <Route exact path="/" element={<Login handleLoginState={setLogin}/>}/>}
            {login &&  <Route exact path="/" element={<FullCalendar/>}/>}           
          </Routes>
               
        </Container>      
      </BrowserRouter>     
    </>
  );
}

export default App;
