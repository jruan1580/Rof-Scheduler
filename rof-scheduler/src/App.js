import NavigationBar from "./NavigationBar";
import Login from "./Login";
import { Container } from "react-bootstrap";
import { useState } from "react";
import Calendar from "./Calendar";
import { BrowserRouter, Route, Routes, Navigate } from "react-router-dom";
import AccountSettings from "./AccountSettings";
import CreateClient from "./CreateClient";
import EmployeeManagement from "./EmployeeManagement";
import ClientManagement from "./ClientManagement";
import PetManagement from "./PetManagement";
import Holidaymanagement from "./HolidayManagement/Holiday/index";
import PetService from "./PetServiceManagement";
import RevenueSummary from "./RevenueSummary";
import PayrollSummary from "./PayrollSummary";


function App() {
  const [isLogin, setLogin] = useState(localStorage.getItem("id") != undefined && localStorage.getItem("firstName") != undefined && localStorage.getItem("role") != undefined);

  return (
    <>
      <BrowserRouter>
        <NavigationBar loginState={isLogin} handleLoginState={setLogin} />
        <br />
        
        <Container> 
          <Routes>
            {/* {!isLogin && (
              <Route
                exact
                path="/"
                element={<Login handleLoginState={setLogin} />}
              />
            )} */}
            {/* {isLogin &&  <Route exact path="/" element={<Calendar/>}/>}            */}                        
            <Route exact path="/" element={!isLogin ? <Login handleLoginState={setLogin} /> : <Calendar setLoginState={setLogin}/>}/>
            <Route exact path="/signup" element={<CreateClient />} />
            <Route exact path="/accountsettings" element={!isLogin ? <Navigate to="/"/> : <AccountSettings setLoginState={setLogin}/>} />
            {/* <Route exact path="/calendar" element={<Calendar />} /> */}
            <Route exact path="/employeemanagement" element={!isLogin ? <Navigate to="/"/> : <EmployeeManagement setLoginState={setLogin}/>} />
            <Route exact path="/clientmanagement" element={!isLogin ? <Navigate to="/"/> : <ClientManagement setLoginState={setLogin}/> } />
            <Route exact path="/petmanagement" element={!isLogin ? <Navigate to="/" /> : <PetManagement setLoginState={setLogin} />} />
            <Route exact path="/holidaymanagement" element={!isLogin ? <Navigate to="/" /> : <Holidaymanagement setLoginState={setLogin} />} />
            <Route exact path="/petservicemanagement" element={!isLogin ? <Navigate to="/" /> : <PetService setLoginState={setLogin} />} />
            <Route exact path="/revenuesummary" element={!isLogin ? <Navigate to="/" /> : <RevenueSummary setLoginState={setLogin} />} />
            <Route exact path="/payrollsummary" element={!isLogin ? <Navigate to="/" /> : <PayrollSummary setLoginState={setLogin} />} />
          </Routes>
        </Container>
      </BrowserRouter>
    </>
  );
}

export default App;
