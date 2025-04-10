$(function(){
    const fileUrl = 'https://api.github.com/repos/gauravt-cf/AutomateCore/contents/AutomateCore/Sample/Program.cs';

  fetch(fileUrl)
    .then(response => response.json())
    .then(data => {
      const content = atob(data.content); // Decode from base64
      document.getElementById('snippet-container').textContent = content;
    })
    .catch(error => {
      console.error('Error loading snippet:', error);
      document.getElementById('snippet-container').textContent = 'Failed to load snippet.';
    });
})