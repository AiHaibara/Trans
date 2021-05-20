from app import app
from livereload import Server

app.debug=True
server = Server(app.wsgi_app)
server.serve()