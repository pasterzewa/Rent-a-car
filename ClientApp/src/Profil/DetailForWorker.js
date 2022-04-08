import React, { Component } from 'react';
import {
    Button, Modal, ModalFooter,
    ModalHeader, ModalBody
} from 'reactstrap';

import 'antd/dist/antd.css';
import { Comment, Avatar } from 'antd';
import { saveAs } from "file-saver";


export default class DetailWorker extends Component {
    constructor(props) {
        super(props);
    }



    render() {

        const saveFile = () => {
            saveAs(
                "https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf",
                "protocol.pdf"
            )
        };
            return (
                <Modal
                    {...this.props}
                    aria-labelledby="contained-modal-title-vcenter"
                    centered
                >
                    <ModalHeader >
                        <p id="contained-modal-title-vcenter">
                            Szczegóły zamówienia
                        </p>
                    </ModalHeader>
                    <ModalBody>
                        <Comment
                    
                            author={<a style={{ fontSize: 15 }}>{this.props.workername}</a>}
                            avatar={<Avatar src="https://joeschmoe.io/api/v1/random" alt="Han Solo" />}
                            content={
                                <p style={{ fontSize: 15}}>
                                    {this.props.carcondition}
                                </p>
                            }
                        />

                        <Button onClick={saveFile}>
                            Pobierz protokół
                        </Button>

                    </ModalBody>

                    <ModalFooter>
                        <Button variant="danger" onClick={this.props.onHide}>Zamknij</Button>
                    </ModalFooter>

                </Modal>
            )
        
    }
}