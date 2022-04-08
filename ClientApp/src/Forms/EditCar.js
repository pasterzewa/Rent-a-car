import React,{Component} from 'react';
import {
    Modal, ModalFooter, Label,
    ModalHeader, ModalBody, Button, Row, Col, Form, FormGroup, Input
} from 'reactstrap';

export default class EditCar extends Component{
    constructor(props){
        super(props);
        this.handleSubmit=this.handleSubmit.bind(this);
    }

    handleSubmit(event){
        event.preventDefault();
        fetch(process.env.REACT_APP_API +'/JsonCars/Edit',{
            method:'POST',
            headers:{
                'Accept':'application/json',
                'Content-Type':'application/json'
            },
            body:JSON.stringify({
                carID:event.target.carID.value,
                Brand:event.target.Brand.value,
                Model:event.target.Model.value,
                HorsePower:event.target.HorsePower.value
            })
        })
        .then(res=>res.json())
        .then((result)=>{
            alert(result);
        },
        (error)=>{
            alert('Failed');
        })
    }
    render(){
        return (
            <div className="container">

<Modal
{...this.props}
size="lg"
aria-labelledby="contained-modal-title-vcenter"
centered
>
    <ModalHeader >
        <p id="contained-modal-title-vcenter">
            Edit Car
        </p>
    </ModalHeader>
    <ModalBody>

        <Row>
            <Col sm={6}>
                <Form onSubmit={this.handleSubmit}>
                <FormGroup controlId="carID">
                        <Label>carID</Label>
                        <Input type="text" name="carID" required
                        disabled
                        defaultValue={this.props.carID} 
                        placeholder="carID"/>
                    </FormGroup>

                    <FormGroup controlId="Brand">
                        <Label>Brand</Label>
                        <Input type="text" name="Brand" required
                        defaultValue={this.props.carbrand}
                        placeholder="Brand"/>
                    </FormGroup>

                    <FormGroup controlId="Model">
                        <Label>Model</Label>
                        <Input type="text" name="Model" required
                        defaultValue={this.props.carmodel}
                        placeholder="Model"/>
                    </FormGroup>

                    <FormGroup controlId="HorsePower">
                        <Label>HorsePower</Label>
                        <Input type="number" min={0} required name="HorsePower" 
                        defaultValue={this.props.carhorsePower}
                        placeholder="HorsePower"/>
                    </FormGroup>

                    <FormGroup>
                        <Button variant="primary" type="submit">
                            Zapisz zmiany
                        </Button>
                    </FormGroup>
                </Form>
            </Col>
        </Row>
    </ModalBody>
    
    <ModalFooter>
        <Button variant="danger" onClick={this.props.onHide}>Close</Button>
    </ModalFooter>

</Modal>

            </div>
        )
    }

}