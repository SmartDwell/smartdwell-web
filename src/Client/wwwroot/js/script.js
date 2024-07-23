window.initMenuToggle = () => {
  const menuToggle = document.querySelector('#menuToggle');
  const sidebar = document.querySelector('#sidebar');
  const content = document.querySelector('.content');

  menuToggle.addEventListener('click', function () {
    sidebar.classList.toggle('open');
    content.classList.toggle('shifted');
  });
};

window.getCurrentUrl = () => {
  return window.location.pathname;
}