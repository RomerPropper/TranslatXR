FROM tiangolo/uvicorn-gunicorn-fastapi:3.11

COPY ./requirements.txt /app/requirements.txt

COPY ./server_prod.py /app/main.py

RUN pip install --no-cache-dir --upgrade -r /app/requirements.txt

