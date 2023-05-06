export { getData, postData }

const fetchMode = 'cors';
const credentialsOption = 'same-origin';

const getData = async (url = '') => {
  return await fetch(url, {
    method: 'GET',
    mode: fetchMode,
    cache: 'no-cache',
    credentials: credentialsOption,
    headers: {
      'Content-Type': 'application/json'
    },
  });
}

const postData = async (url = '', data = {}) => {
  return await fetch(url, {
    method: 'POST',
    mode: fetchMode,
    cache: 'no-cache',
    credentials: credentialsOption,
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(data)
  });
}