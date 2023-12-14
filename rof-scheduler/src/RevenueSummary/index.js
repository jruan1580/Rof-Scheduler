import { Row, Form, Col, Button, Tabs, Tab } from "react-bootstrap";
import { useState } from "react";
import { ensureDateSearchInformationProvided } from "../SharedServices/inputValidationService";

function RevenueSummary({setLoginState}){
    const [validationMap, setValidationMap] = useState(new Map());

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

            <Tabs
                defaultActiveKey="home"
                id="justify-tab-example"
                className="mb-3"
                justify
            >
                <Tab eventKey="pet" title="Pet">
                    Tab content for Home
                </Tab>
                <Tab eventKey="numOfEvents" title="Number of Events">
                    Tab content for Profile
                </Tab>
                <Tab eventKey="grossRevenue" title="Gross Revenue">
                    Tab content for Loooonger Tab
                </Tab>
                <Tab eventKey="netRevenue" title="Net Revenue">
                    Tab content for Contact
                </Tab>
            </Tabs>
        </>
    );
}

export default RevenueSummary;