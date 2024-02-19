import { Row, Form, Col, Button, Table, Alert, Pagination } from "react-bootstrap";
import { useState } from "react";

function PayrollSummary({setLoginState}){
    const [currPage, setCurrPage] = useState(1);
    const [totalPages, setTotalPages] = useState(0);

    const search = (e) => {
        e.preventDefault();        
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
                        <td>John</td>
                        <td>Doe</td>
                        <td>20</td>
                    </tr>
                </tbody>
            </Table>
            <Pagination>
                {currPage != 1 && (
                <Pagination.Prev onClick={() => setCurrPage(currPage - 1)} />
                )}
                {currPage != totalPages && (
                <Pagination.Next onClick={() => setCurrPage(currPage + 1)} />
                )}
            </Pagination>
        </>
    );
}

export default PayrollSummary;