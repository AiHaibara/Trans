from functools import wraps
from flask import g, request, jsonify, redirect, url_for
def api_key(f):
    @wraps(f)
    def decorator(*args, **kwargs):

        token = None

        if 'x-api-key' in request.headers:
            token = request.headers['x-api-key']

        if not token:
            return jsonify({'message': 'a valid token is missing'})

        try:
            valid=token==app.secret_key
            return valid
        #  data = jwt.decode(token, app.config[SECRET_KEY])
        #  current_user = Users.query.filter_by(public_id=data['public_id']).first()
        except:
            return jsonify({'message': 'token is invalid'})

        return f(*args, **kwargs)
    return decorator

# def jwt(f):
#    @wraps(f)
#    def decorator(*args, **kwargs):

#       token = None

#       if 'x-access-tokens' in request.headers:
#          token = request.headers['x-access-tokens']

#       if not token:
#          return jsonify({'message': 'a valid token is missing'})

#       try:
#          data = jwt.decode(token, app.config[SECRET_KEY])
#          current_user = Users.query.filter_by(public_id=data['public_id']).first()
#       except:
#          return jsonify({'message': 'token is invalid'})

#         return f(current_user, *args, **kwargs)
#    return decorator