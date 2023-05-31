import { Box, Button, Heading, Input } from '@chakra-ui/react';
import React, { useState } from 'react';

export const Home = () => {
  const storedToken = sessionStorage.getItem('token');

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [token, setToken] = useState('');

  const HandleLogin = () => {
    const loginData = {
      email: email,
      password: password,
    };

    fetch(`${process.env.REACT_APP_BACKEND_URL}/Auth/Login`, {
      method: 'post',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(loginData),
    })
      .then(response => response.json())
      .then(body => {
        if (body.token) {
          sessionStorage.setItem('token', body.token);
          setToken(body.token);
        } else {
          console.log(body.errorMessage);
        }
      })
      .catch(err => console.log(err));
  };

  const HandleGetSecureImages = () => {
    if (!token) {
      console.log('You dont have a token');
      return;
    }

    fetch(`${process.env.REACT_APP_BACKEND_URL}/SuperSecureImage/GetImages`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    }).then(res => {
      if (res.status === 401) {
        console.log('You are not logged in');
        return;
      }
      if (res.status === 403) {
        console.log('You are not admin');
        return;
      }
      if (res.ok) {
        console.log('You made it buddy');
      }
    });
    // .catch(error => console.log(error))
  };

  if (!storedToken)
    return (
      <Box>
        <Heading>Login</Heading>
        <Input
          onChange={e => setEmail(e.target.value)}
          type={'email'}
          placeholder="Please enter your email"
        />
        <Input
          onChange={e => setPassword(e.target.value)}
          placeholder="Please enter your password"
        />
        <Button onClick={HandleLogin}>Login</Button>
        <hr />
      </Box>
    );
  else
    return (
      <Box>
        You're already logged in
        <Button onClick={HandleGetSecureImages}>Get Secure Images</Button>
      </Box>
    );
};
