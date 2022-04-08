import React,{Component} from 'react';
import { Form, Table, Col, Row, FormGroup, Label, Input} from 'reactstrap';

import {Button,ButtonToolbar} from 'reactstrap';
import  EditCar  from './Forms/EditCar';
import CarDetalisWindow from './CarDetalisWindow';

export default class Cars extends Component{

    constructor(props){
        super(props);
        this.state={
            cars:[],
            editModalShow:false,
            carDetalisModalShow:false,
            Brand:null,
            Model:null,
            order: 0,
            carsToShow: [],
            AlienCars: []
        }

        this.handleSubmit=this.handleSubmit.bind(this);
    }

    filter() {
        this.setState({
            carsToShow: this.state.cars.concat(this.state.AlienCars.map(item => {
                return {
                    CarID: item.id,
                    Brand: item.brandName,
                    Model: item.modelName,
                    HorsePower: item.enginePower,
                    Api: "Alien"
                }
            })).filter(a =>
                (a.Brand == this.state.Brand || this.state.Brand == null) &&
                (a.Model == this.state.Model || this.state.Model == null)).sort((a, b) => {
                    if (this.state.order === 0) {
                        if (a.Brand === b.Brand)
                            return a.Model >= b.Model ? 1 : -1;
                        else
                            return a.Brand >= b.Brand ? 1 : -1;
                    }
                    else {
                        if (a.Model === b.Model)
                            return a.Brand >= b.Brand ? 1 : -1;
                        else
                            return a.Model >= b.Model ? 1 : -1;
                    }
                })
        });
        console.log(this.state.carsToShow);
    }


    refreshList() {
        fetch(process.env.REACT_APP_API + '/JsonCars')
            .then(response => response.json())
            .then(data => {

                this.setState({
                    cars: data.map(item => {
                        return {
                            CarID: item.CarID,
                            Brand: item.Brand,
                            Model: item.Model,
                            HorsePower: item.HorsePower,
                            Api: "OurApi"
                        }
                    })
                }, this.filter());
            });

        fetch(process.env.REACT_APP_API + '/AlienApi/Get')
            .then(response => response.json())
            .then(data => {
                this.setState({ AlienCars: data.vehicles }, this.filter());

            });
    }
    


    handleSubmit(event){
        event.preventDefault();
        var m = event.target.Model.value;
        if(m==='')
            m = null;
        var b = event.target.Brand.value;
        if(b === '')
            b = null;
        console.log(this.state);
        this.setState(
        {
            Model: m,
            Brand: b
            }, () => this.filter());

        
    }


    componentDidMount(){
        this.refreshList();
        this.refreshList();
        
    }

    componentWillUnmount() {
    }


    render(){
        const { carsToShow, carID, messengFromAlien, Api}=this.state;
        let CarDetalisModalClose=()=>this.setState({carDetalisModalShow:false});
        return(
            <div >
                <div>
                    <Form onSubmit={this.handleSubmit}>
                        <Col sm={4}>
                            <FormGroup controlId="Brand">
                                <Label>Firma</Label>
                                <Input type="text" name="Brand"
                                    placeholder="Firma" />
                            </FormGroup>

                            <FormGroup controlId="Model">
                                <Label>Model</Label>
                                <Input type="text" name="Model"
                                    placeholder="Model" />
                            </FormGroup>
                        </Col>

                        <Row>

                            <FormGroup>
                                <Button variant="primary" type="submit">
                                    Szukaj
                                </Button>

                                <Button variant="secondary"
                                    onClick={() => {
                                        if (this.state.order === 0)
                                            this.setState({ order: 1 }, this.filter())
                                        else
                                            this.setState({ order: 0 }, this.filter())
                                        
                                    }

                                    }>
                                    Zmień kolejność wyświetlania
                                </Button>

                            </FormGroup>




                        </Row>

                    </Form>
                </div>
                <Table className="mt-4" striped bordered hover size="sm">
                    <thead>
                        <tr>
                            <th>Brand</th>
                            <th>Model</th>
                            <th>HorsePower</th>
                        </tr>
                    </thead>
                    <tbody>
                        {carsToShow.map(car=>
                            <tr key={car.CarID}>
                                <td>{car.Brand}</td>
                                <td>{car.Model}</td>
                                <td>{car.HorsePower}</td>
                                <td> 
                                    <ButtonToolbar>
                                      
                                            <Button className="mr-2" variant="info"
                                            onClick={()=>this.setState(
                                            {
                                                carDetalisModalShow:true,
                                                    carID: car.CarID,
                                                    messengFromAlien: car.Api === "Alien" ? this.state.AlienCars.find(item => item.CarID === carID) : this.state.AlienCars[0],
                                                    Api: car.Api
                                            })}>
                                                Szczegóły
                                            </Button>

                                            
                                        <CarDetalisWindow isOpen={this.state.carDetalisModalShow}
                                            onHide={CarDetalisModalClose}
                                            id={carID}
                                            api={Api}
                                            messengFromAlien={messengFromAlien}
                                            
                                        />




                                    </ButtonToolbar>
                                    

                                </td>

                            </tr>
                            
                        )}
                    </tbody>

                </Table>

            </div>
        )
    }
}