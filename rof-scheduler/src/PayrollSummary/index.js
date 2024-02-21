import { Row, Form, Col, Button, Table, Alert, Pagination } from "react-bootstrap";
import { useState } from "react";
import { ensureDateSearchInformationProvided } from "../SharedServices/inputValidationService";
import { getPayrollBetweenDatesByEmployee } from "../SharedServices/datamartService";

function PayrollSummary({setLoginState}){
    const [validationMap, setValidationMap] = useState(new Map());
    const [errorMessage, setErrorMessage] = useState(undefined);
    const [showTable, setShowTable] = useState(false);
    const [currPage, setCurrPage] = useState(1);
    const [totalPages, setTotalPages] = useState(0);
    const [payrollSummaryByEmployee, setPayrollSummaryByEmployee] = useState([]);

    const search = (e) => {
        e.preventDefault(); 

        var firstName = e.target.firstName.value;
        var lastName = e.target.lastName.value;
        var startDate = e.target.startDate.value;
        var endDate = e.target.endDate.value;

        var inputValidations = ensureDateSearchInformationProvided (startDate, endDate);
            if (inputValidations.size > 0) {
                setValidationMap(inputValidations);
                return;
            }

        setValidationMap(new Map());

        (async function () {
            try {
                const resp = await getPayrollBetweenDatesByEmployee(firstName, lastName, startDate, endDate, currPage);
                
                if (resp.status === 401){
                    setLoginState(false);
                    return;
                }

                const payrollSumWithPage = await resp.json();

                setPayrollSummaryByEmployee(payrollSumWithPage.payrollPerEmployee);
                setTotalPages(payrollSumWithPage.totalPages);
            } catch (e) {
                setErrorMessage(e.message);
            }
            })();        
            
            if(payrollSumWithPage === undefined || payrollSumWithPage === null){
                setErrorMessage("No payroll summary found for employee for this period.");
                setShowTable(false);
            }else{
                setErrorMessage(undefined);
                setShowTable(true);
            }

        setShowTable(true);       
    };
    
    return(
        <>
        <h1>Payroll Summary</h1>
        <br />
        <Form onSubmit={search}>
                <Row>
                    <Form.Group as={Col} lg={6}>
                        <Form.Label>First Name:</Form.Label>
                        <Col lg={12}>
                            <Form.Control
                                type="text"
                                name="firstName"
                                placeholder="First Name"
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Col} lg={6}>
                        <Form.Label>Last Name:</Form.Label>
                        <Col lg={12}>
                            <Form.Control
                                type="text"
                                name="lastName"
                                placeholder="Last Name"
                            />
                        </Col>
                    </Form.Group>
                </Row>
                <br />
                <Row>
                    <Form.Group as={Col} lg={6}>
                        <Form.Label>Start Date:</Form.Label>
                        <Col lg={12}>
                            <Form.Control
                                type="date"
                                name="startDate"
                                defaultValue= ""
                            />
                            <Form.Control.Feedback type="invalid">
                                {validationMap.get("startDate")}
                            </Form.Control.Feedback>
                        </Col>
                    </Form.Group>
                    <Form.Group as={Col} lg={6}>
                        <Form.Label>End Date:</Form.Label>
                        <Col lg={12}>
                            <Form.Control
                                type="date"
                                name="endDate"
                                defaultValue= ""
                            />
                            <Form.Control.Feedback type="invalid">
                                {validationMap.get("endDate")}
                            </Form.Control.Feedback>
                        </Col>
                    </Form.Group>
                </Row>
                <br />
                <Button type="submit" variant="primary" className="float-end ms-2">Search</Button>
            </Form>
            <br />
            <br />
            <hr />
            <br />

            {showTable && 
                <Table responsive striped bordered>
                    <thead>
                        <tr>
                            <th>Last Name</th>
                            <th>First Name</th>
                            <th>Total Pay</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                    </tbody>
                </Table>
            }
            {showTable && 
                <Pagination>
                    {currPage != 1 && (
                        <Pagination.Prev onClick={() => setCurrPage(currPage - 1)} />
                    )}
                    {currPage != totalPages && (
                        <Pagination.Next onClick={() => setCurrPage(currPage + 1)} />
                    )}
                </Pagination>
            }
        </>
    );
}

export default PayrollSummary;