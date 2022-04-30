import NavigationBar from './NavigationBar';
import Login from './Login';
import { Container } from 'react-bootstrap';
import { useEffect, useState } from 'react';
import Calendar from './Calendar';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import AccountSettings from './AccountSettings';

function App() {
  const [login, setLogin] = useState(false);

  useEffect(() => {
    console.log(localStorage.getItem("Id"));

    if (localStorage.getItem("id") != undefined && localStorage.getItem("firstName") != undefined){
      setLogin(true);
    }
  }, []);

  return (
    <>
      <BrowserRouter>
        <NavigationBar loginState={login} handleLoginState={setLogin}/>
        <br/>
        <Container>
          <Routes>
            {!isLogin &&  <Route exact path="/" element={<Login handleLoginState={setLogin}/>}/>}
            {/* {isLogin &&  <Route exact path="/" element={<Calendar/>}/>}            */}
          </Routes>
          <Routes>
          <Route exact path="/calendar" element={<Calendar/>}/>        
          </Routes>
          <Routes>
            <Route exact path="/accountsettings" element={<AccountSettings/>}/>
          </Routes>
        </Container>      
      </BrowserRouter>     
    </>
  );
}

export default App;
