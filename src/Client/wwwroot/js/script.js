window.initMenuToggle = () => {
  console.log('initMenuToggle');
  const menuToggle = document.querySelector('#menuToggle');
  const sidebar = document.querySelector('#sidebar');
  const content = document.querySelector('.content');

  menuToggle.addEventListener('click', function () {
    console.log('menuToggle clicked');
    sidebar.classList.toggle('open');
    content.classList.toggle('shifted');
  });
};