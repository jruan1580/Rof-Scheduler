import { Row, Form, Col, Button, Table } from "react-bootstrap";
import { useState } from "react";
import { ensureDateSearchInformationProvided } from "../SharedServices/inputValidationService";
import { getRevenueBetweenDatesByPetService } from "../SharedServices/datamartService";

function RevenueSummary({setLoginState}){
    const [validationMap, setValidationMap] = useState(new Map());
    const [showTable, setShowTable] = useState(false);
    const [revSummary, setRevSummary] = useState([]);

    const search = (e) => {
        e.preventDefault();

        var startDate = e.target.startDate.value;
        var endDate = e.target.endDate.value;

        var inputValidations = ensureDateSearchInformationProvided (startDate, endDate);
            if (inputValidations.size > 0) {
                setValidationMap(inputValidations);
                console.log(inputValidations);
                return;
            }

        setValidationMap(new Map());

        //get request
        setShowTable(true);
    };

    return(
        <>
            <h1>REVENUE SUMMARY</h1>
            <br />
            <Form onSubmit={search}>
                <Row>
                    <Form.Group as={Col} lg={6}>
                        <Form.Label>Start Date:</Form.Label>
                        <Col lg={12}>
                            <Form.Control
                                type="date"
                                name="startDate"
                                defaultValue= ""
                                isInvalid={validationMap.has("startDate")}
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
                                isInvalid={validationMap.has("endDate")}
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
                            <th>Pet Service</th>
                            <th>Number of Events</th>
                            <th>Gross Revenue</th>
                            <th>Net Revenue</th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            revSummary.length != 0 &&
                            revSummary.map((summary) => {
                                return(
                                    <tr key = {summary.id}>
                                        <td></td>
                                    </tr>
                                )
                            })
                        }
                    </tbody>
                </Table>
            }
        </>
    );
}

export default RevenueSummary;